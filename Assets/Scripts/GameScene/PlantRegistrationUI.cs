using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantRegistrationUI : MonoBehaviour
{
    [Header("Popup")]
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private Button confirmButton;

    [Header("List")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject rowPrefab;
    [SerializeField] private List<PlantData> selectablePlants;

    private List<PlantSelectionEntry> spawnedEntries = new List<PlantSelectionEntry>();
    private Action<List<string>> onConfirmCallback;

    private void Awake()
    {
        popupRoot.SetActive(false);
        confirmButton.onClick.AddListener(ConfirmSelection);
    }

    public void Show(Action<List<string>> onConfirm)
    {
        onConfirmCallback = onConfirm;
        popupRoot.SetActive(true);

        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        spawnedEntries.Clear();

        foreach (PlantData plant in selectablePlants)
        {
            GameObject rowObj = Instantiate(rowPrefab, contentParent);
            PlantSelectionEntry entry = rowObj.GetComponent<PlantSelectionEntry>();
            entry.Setup(plant);
            spawnedEntries.Add(entry);
        }
    }

    private void ConfirmSelection()
    {
        List<string> selectedPlantIds = new List<string>();

        foreach (PlantSelectionEntry entry in spawnedEntries)
        {
            if (entry.IsSelected())
                selectedPlantIds.Add(entry.GetPlantId());
        }

        if (selectedPlantIds.Count == 0)
        {
            Debug.Log("Player must select at least one plant.");
            return;
        }

        popupRoot.SetActive(false);
        onConfirmCallback?.Invoke(selectedPlantIds);
    }
}