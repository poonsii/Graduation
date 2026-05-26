using UnityEngine;
using UnityEngine.UI;

public class PlantToggleBinding : MonoBehaviour
{
    public string plantId;
    public Toggle toggle;

    public bool IsSelected()
    {
        return toggle != null && toggle.isOn;
    }
}
