using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private PlayerProfileManager profileManager; // save/load system.
    [SerializeField] private PlantDatabase plantDatabase;
    [SerializeField] private InventoryManager inventoryManager; // owned plants list.
    [SerializeField] private PlantCardSpawner cardSpawner; // creates plant cards.
    [SerializeField] private PlantRegistrationUI registrationUI; // registration popup.
    [SerializeField] private PlantWorldDisplay plantWorldDisplay;
    [SerializeField] private PlantOnboardingFlowController onboardingFlowController; // walks the player through each plant one by one.
    [SerializeField] private PlantLocationSelector locationSelector; // used to look up spot positions for plants that already have a saved location.

    private bool registrationRequired; // checks if registration is needed.
    private PlayerData currentData; // keeps the current save data in memory.

    private void Start()
    {
        LocalizationSettings.InitializationOperation.WaitForCompletion(); // make sure locales/tables are ready before any onboarding text is requested.

        currentData = profileManager.Load(); // load saved data.

        if (currentData == null)
            currentData = new PlayerData(); // make new save data if nothing exists yet.

        if (!currentData.hasFinishedPlantRegistration || currentData.ownedPlantIds == null || currentData.ownedPlantIds.Count == 0)
        {
            ForcePlantRegistration(); // show popup if no plants are saved.
        }
        else
        {
            LoadOwnedPlants(currentData); // load saved plants.
            HideUnplacedPlants();
            RestorePlacedPlantPositions();
            BeginOnboardingIfNeeded();
        }
    }

    private void OnPlantsChosen(List<string> selectedPlantIds)
    {
        if (selectedPlantIds == null || selectedPlantIds.Count == 0)
        {
            Debug.LogWarning("You must select at least one plant.");
            ForcePlantRegistration();
            return;
        }

        currentData = new PlayerData(); // fresh save data.
        currentData.hasFinishedPlantRegistration = true; // mark registration done.
        currentData.ownedPlantIds = new List<string>(selectedPlantIds); // save chosen plant ids.

        currentData.savedPlants = new List<SavedPlantState>(); // create the saved plant states list.

        foreach (string plantId in selectedPlantIds)
        {
            SavedPlantState plantState = new SavedPlantState();
            plantState.uniquePlantInstanceId = System.Guid.NewGuid().ToString(); // create a unique id for this exact plant.
            plantState.plantId = plantId; // save which plant type it is.
            plantState.lastCareTime = "";
            plantState.isUnhealthy = false;
            plantState.hasCompletedOnboarding = false; // onboarding is not done yet.

            currentData.savedPlants.Add(plantState); // add this plant state to the save data.
        }

        profileManager.Save(currentData); // write save file.
        registrationRequired = false; // registration not needed anymore.
        LoadOwnedPlants(currentData); // load the plants.
        HideUnplacedPlants();
        BeginOnboardingIfNeeded();
    }

    private void HideUnplacedPlants()
    {
        if (currentData == null || currentData.savedPlants == null)
            return;

        foreach (SavedPlantState plantState in currentData.savedPlants)
        {
            if (plantState.selectedLightLocation == LightLocationType.Unknown)
                inventoryManager.SetRoomPlantVisible(plantState.plantId, false); // stay hidden until the player picks a location.
        }
    }

    private void RestorePlacedPlantPositions()
    {
        if (currentData == null || currentData.savedPlants == null || locationSelector == null)
            return;

        foreach (SavedPlantState plantState in currentData.savedPlants)
        {
            if (plantState.selectedLightLocation == LightLocationType.Unknown)
                continue;

            // prefer the exact spot; fall back to matching by light type for saves made before spot ids existed.
            Transform spotTransform = !string.IsNullOrEmpty(plantState.selectedSpotId)
                ? locationSelector.GetSpotTransformById(plantState.selectedSpotId)
                : locationSelector.GetSpotTransform(plantState.selectedLightLocation);

            if (spotTransform != null)
                inventoryManager.MovePlantToSpot(plantState.plantId, spotTransform);
        }
    }

    public bool IsSpotTaken(string spotId, string excludingInstanceId)
    {
        if (string.IsNullOrEmpty(spotId) || currentData == null || currentData.savedPlants == null)
            return false;

        foreach (SavedPlantState plantState in currentData.savedPlants)
        {
            if (plantState.uniquePlantInstanceId == excludingInstanceId)
                continue;

            if (plantState.selectedSpotId == spotId)
                return true;
        }

        return false;
    }

    private void BeginOnboardingIfNeeded()
    {
        if (currentData == null || currentData.savedPlants == null || onboardingFlowController == null)
        {
            Debug.LogWarning("[Onboarding] Stopped early - currentData null: " + (currentData == null)
                + ", savedPlants null: " + (currentData?.savedPlants == null)
                + ", onboardingFlowController assigned: " + (onboardingFlowController != null));
            return;
        }

        List<SavedPlantState> pending = currentData.savedPlants.FindAll(p => !p.hasCompletedOnboarding); // plants still missing location or care info.

        Debug.Log("[Onboarding] Plants still needing onboarding: " + pending.Count);

        if (pending.Count > 0)
            onboardingFlowController.BeginOnboarding(pending); // walk the player through them one by one.
    }

    public void UpdatePlantLocation(
    string uniquePlantInstanceId,
    LightLocationType selectedLightLocation,
    bool playerAcceptedMismatch,
    Transform spotTransform,
    string spotId)
    {
        SavedPlantState plantState = FindPlantState(uniquePlantInstanceId);
        if (plantState == null)
            return;

        PlantData plant = plantDatabase.GetById(plantState.plantId); // get the plant data for this saved plant.

        if (plant == null)
        {
            Debug.LogWarning("Could not find plant data for id: " + plantState.plantId);
            return;
        }

        plantState.selectedLightLocation = selectedLightLocation; // save chosen light location.
        plantState.selectedSpotId = spotId; // save exactly which spot, so it can't be double-booked or lost among duplicate light types.
        plantState.lightAdviceResult = PlantLocationAdvisor.GetAdvice(plant.requiredLight, selectedLightLocation); // calculate location advice from plant need.
        plantState.playerAcceptedMismatch = playerAcceptedMismatch; // save if the player ignored the advice.

        inventoryManager.MovePlantToSpot(plantState.plantId, spotTransform); // physically place the plant at the chosen spot.
        inventoryManager.SetRoomPlantVisible(plantState.plantId, true); // reveal it now that it has a confirmed location.

        RecalculateOnboardingComplete(plantState);
        profileManager.Save(currentData); // save updated plant state.
        inventoryManager.RefreshPlantCards(); // update the plant card with the new location info.
        inventoryManager.RefreshRoomPlantCardInfo(); // update the room plant's card with the new location info.
    }

    public void PreviewPlantLocation(string plantId, Transform spotTransform)
    {
        inventoryManager.MovePlantToSpot(plantId, spotTransform);
        inventoryManager.SetRoomPlantVisible(plantId, true);
    }

    public void SetPlantCardInteractable(string plantId, bool interactable)
    {
        inventoryManager.SetPlantCardInteractable(plantId, interactable);
    }

    public void HidePlantPreview(string plantId)
    {
        inventoryManager.SetRoomPlantVisible(plantId, false);
    }

    private void RecalculateOnboardingComplete(SavedPlantState plantState)
    {
        // the plant still needs onboarding until both the location and the calendar step are done.
        plantState.hasCompletedOnboarding = plantState.selectedLightLocation != LightLocationType.Unknown
            && plantState.hasCompletedCalendarIntro;
    }

    private SavedPlantState FindPlantState(string uniquePlantInstanceId)
    {
        if (currentData == null || currentData.savedPlants == null)
        {
            Debug.LogWarning("Could not find saved plant state for id: " + uniquePlantInstanceId);
            return null; // stop if there is no loaded save data.
        }

        SavedPlantState plantState = currentData.savedPlants.Find(p => p.uniquePlantInstanceId == uniquePlantInstanceId);

        if (plantState == null)
            Debug.LogWarning("Could not find saved plant state for id: " + uniquePlantInstanceId);

        return plantState;
    }

    public SavedPlantState GetSavedPlantState(string uniquePlantInstanceId)
    {
        if (currentData == null || currentData.savedPlants == null)
            return null; // stop if there is no loaded save data.

        return currentData.savedPlants.Find(p => p.uniquePlantInstanceId == uniquePlantInstanceId); // return the exact saved plant.
    }

    public SavedPlantState GetSavedPlantStateForPlant(string plantId)
    {
        if (currentData == null || currentData.savedPlants == null)
            return null; // stop if there is no loaded save data.

        return currentData.savedPlants.Find(p => p.plantId == plantId); // return the saved state for this plant type (used by the plant cards).
    }

    public PlantData GetPlantData(string plantId)
    {
        return plantDatabase != null ? plantDatabase.GetById(plantId) : null;
    }

    public void MarkCalendarIntroSeen(string uniquePlantInstanceId)
    {
        SavedPlantState plantState = FindPlantState(uniquePlantInstanceId);
        if (plantState == null)
            return;

        plantState.hasCompletedCalendarIntro = true; // remember this step was shown, whether the player logged something or skipped it.
        RecalculateOnboardingComplete(plantState);
        profileManager.Save(currentData);
    }

    public List<string> LogPlantCare(string plantId, CareActionType actionType, int pointsEarned)
    {
        SavedPlantState plantState = GetSavedPlantStateForPlant(plantId);
        if (plantState == null)
            return new List<string>();

        string todayString = CalendarClock.Now.ToString("yyyy-MM-dd");

        CalendarLogEntry entry = new CalendarLogEntry();
        entry.date = todayString;
        entry.actionType = actionType;
        entry.pointsEarned = pointsEarned;
        plantState.careLog.Add(entry); // keep a full history of logged care.

        if (actionType == CareActionType.Watered)
            plantState.lastWateredDate = todayString;
        else
            plantState.lastFertilizedDate = todayString;

        List<string> newlyEarnedBadges = CalendarBadgeManager.RefreshBadges(plantState, plantDatabase.GetById(plantId), CalendarClock.Now);

        if (PointsManager.Instance != null)
            PointsManager.Instance.AddPoints(pointsEarned); // actually award the points the calendar just computed.

        profileManager.Save(currentData); // persist the new log, dates and badges.
        inventoryManager.RefreshPlantCards(); // let the plant card show the new last-watered date.
        inventoryManager.RefreshRoomPlantCardInfo(); // let the 3D room plant show the new last-watered date.

        return newlyEarnedBadges;
    }

    public bool UnlogPlantCare(string plantId, CareActionType actionType)
    {
        SavedPlantState plantState = GetSavedPlantStateForPlant(plantId);
        if (plantState == null)
            return false;

        string todayString = CalendarClock.Now.ToString("yyyy-MM-dd");

        CalendarLogEntry entry = plantState.careLog.Find(e => e.date == todayString && e.actionType == actionType);
        if (entry == null)
            return false; // nothing logged today for this action - nothing to undo.

        plantState.careLog.Remove(entry);

        if (PointsManager.Instance != null)
            PointsManager.Instance.AddPoints(-entry.pointsEarned); // undo the reward this entry gave.

        RecomputeLastActionDate(plantState, actionType);

        profileManager.Save(currentData);
        inventoryManager.RefreshPlantCards();
        inventoryManager.RefreshRoomPlantCardInfo();

        return true;
    }

    private void RecomputeLastActionDate(SavedPlantState plantState, CareActionType actionType)
    {
        string mostRecentDate = ""; // yyyy-MM-dd strings sort correctly as plain text.

        foreach (CalendarLogEntry entry in plantState.careLog)
        {
            if (entry.actionType == actionType && string.Compare(entry.date, mostRecentDate, StringComparison.Ordinal) > 0)
                mostRecentDate = entry.date;
        }

        if (actionType == CareActionType.Watered)
            plantState.lastWateredDate = mostRecentDate;
        else
            plantState.lastFertilizedDate = mostRecentDate;
    }

    public bool RefreshCalendarBadges(string plantId)
    {
        SavedPlantState plantState = GetSavedPlantStateForPlant(plantId);
        if (plantState == null)
            return false;

        List<string> newlyEarned = CalendarBadgeManager.RefreshBadges(plantState, plantDatabase.GetById(plantId), CalendarClock.Now);
        profileManager.Save(currentData); // the streak may have just reset even though nothing was logged.

        return newlyEarned.Count > 0;
    }

    private void LoadOwnedPlants(PlayerData data)
    {
        inventoryManager.Clear(); // clear old owned plants.

        foreach (string plantId in data.ownedPlantIds)
        {
            PlantData plant = plantDatabase.GetById(plantId); // find plant by id.
            if (plant != null)
            {
                inventoryManager.AddPlant(plant); // add to owned list.
            }
        }

        inventoryManager.RefreshRoomPlants(); // update room plants
        inventoryManager.RefreshPlantCards(); // update cards
        inventoryManager.RefreshRoomPlantCardInfo(); // show each plant's last-watered date
        cardSpawner.SpawnCards(data.ownedPlantIds); // spawn owned cards

        if (plantWorldDisplay != null)
            plantWorldDisplay.ShowOwnedPlants(data.ownedPlantIds); // show owned plants in world
    }

    public void ResetPlantsAndRequireRegistration()
    {
        profileManager.DeleteSave(); // delete save file.
        inventoryManager.Clear(); // clear owned plants.
        registrationRequired = true; // require registration again.
        currentData = new PlayerData(); // reset current data too.

        ForcePlantRegistration(); // open popup again.
    }

    private void ForcePlantRegistration()
    {
        registrationRequired = true; // mark registration needed.
        registrationUI.Show(OnPlantsChosen); // show popup and wait for choice
    }
}