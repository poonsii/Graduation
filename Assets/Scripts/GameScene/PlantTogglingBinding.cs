using UnityEngine;
using UnityEngine.UI;

public class PlantToggleBinding : MonoBehaviour
{
    public string plantId;
    public Toggle toggle; // toggle for selecting the plant

    public bool IsSelected()
    {
        return toggle != null && toggle.isOn;
    }
}
