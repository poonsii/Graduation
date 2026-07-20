using UnityEngine;
using UnityEngine.EventSystems;

public class CardsScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject cardsPanel; // cards screen.

    private void Start()
    {
        if (cardsPanel != null)
            cardsPanel.SetActive(false); // start hidden.
    }

    public void OpenCards()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null); // clear selected button

        if (cardsPanel != null)
            cardsPanel.SetActive(true); // open screen
    }

    public void CloseCards()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null); // clear selection again.

        if (cardsPanel != null)
            cardsPanel.SetActive(false); // close screen.
    }
}