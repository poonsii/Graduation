using UnityEngine;
using UnityEngine.EventSystems;

public class CardsScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject cardsPanel;

    private void Start()
    {
        if (cardsPanel != null)
            cardsPanel.SetActive(false);
    }

    public void OpenCards()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        if (cardsPanel != null)
            cardsPanel.SetActive(true);
    }

    public void CloseCards()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        if (cardsPanel != null)
            cardsPanel.SetActive(false);
    }
}