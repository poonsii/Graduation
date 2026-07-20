using UnityEngine;
using UnityEngine.EventSystems;

public class PlantCardButtonOpener : MonoBehaviour
{
    [SerializeField] private GameObject plantCardUI; 

    private void Start()
    {
        if (plantCardUI != null)
            plantCardUI.SetActive(false); // hide the card on start.
    }

    public void OpenPlantCard()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null); 

        if (plantCardUI != null)
            plantCardUI.SetActive(true); // show the plant card.
    }

    public void ClosePlantCard()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null); 

        if (plantCardUI != null)
            plantCardUI.SetActive(false); // hide the plant card.
    }
}