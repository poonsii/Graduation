using System;
using System.Collections.Generic;
using UnityEngine;

public class PlantRegistrationUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private List<PlantToggleBinding> plantToggles;

    private Action<List<string>> onConfirm;

    public void Show(Action<List<string>> confirmCallback)
    {
        onConfirm = confirmCallback;
        panelRoot.SetActive(true);

        foreach (PlantToggleBinding entry in plantToggles)
        {
            if (entry != null && entry.toggle != null)
                entry.toggle.isOn = false;
        }
    }

    public void ConfirmSelection()
    {
        List<string> selectedPlantIds = new List<string>();

        foreach (PlantToggleBinding entry in plantToggles)
        {
            if (entry != null && entry.IsSelected())
            {
                selectedPlantIds.Add(entry.plantId);
            }
        }

        if (selectedPlantIds.Count == 0)
        {
            Debug.LogWarning("Select at least one plant.");
            return;
        }

        panelRoot.SetActive(false);
        onConfirm?.Invoke(selectedPlantIds);
    }
}