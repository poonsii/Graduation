using UnityEngine;
using UnityEngine.EventSystems;

public class OptionsMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel; 

    private void Start()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    public void OpenOptions() // open the options panel.
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }

    public void CloseOptions() // close the options panel.
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }
}