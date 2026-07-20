using System;
using System.Collections.Generic;
using UnityEngine;

public class PlantRegistrationUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot; 
    [SerializeField] private List<PlantToggleBinding> plantToggles; // plant choices in the list.

    private Action<List<string>> onConfirm; 

    public void Show(Action<List<string>> confirmCallback)
    {
        onConfirm = confirmCallback; // store the next action.
        panelRoot.SetActive(true); // show the popup.

        foreach (PlantToggleBinding entry in plantToggles)
        {
            if (entry != null && entry.toggle != null)
                entry.toggle.isOn = false; // reset all choices.
        }
    }

    public void ConfirmSelection()
    {
        List<string> selectedPlantIds = new List<string>(); // chosen plants.

        foreach (PlantToggleBinding entry in plantToggles)
        {
            if (entry != null && entry.IsSelected())
            {
                selectedPlantIds.Add(entry.plantId); // add chosen plant id
            }
        }

        if (selectedPlantIds.Count == 0)
        {
            Debug.LogWarning("select at least one plant."); // warning if nothing was picked.
            return;
        }

        panelRoot.SetActive(false); // hide the popup.
        onConfirm?.Invoke(selectedPlantIds); // send the choices forward
    }
}