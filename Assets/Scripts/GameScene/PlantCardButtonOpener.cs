using UnityEngine;
using UnityEngine.EventSystems;

public class PlantCardButtonOpener : MonoBehaviour
{
    [SerializeField] private GameObject plantCardUI;

    private void Start()
    {
        if (plantCardUI != null)
            plantCardUI.SetActive(false);
    }

    public void OpenPlantCard()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        if (plantCardUI != null)
            plantCardUI.SetActive(true);
    }

    public void ClosePlantCard()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        if (plantCardUI != null)
            plantCardUI.SetActive(false);
    }
}