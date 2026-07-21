using UnityEngine;

public class PlantLocationSpot : MonoBehaviour
{
    public string displayName;
    public LightLocationType lightLocationType = LightLocationType.Unknown;

    [SerializeField] private Renderer[] highlightRenderers;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.green;

    public LightLocationType GetLocationType() => lightLocationType;
    public string GetDisplayName() => string.IsNullOrEmpty(displayName) ? lightLocationType.ToString() : displayName;

    public void SetSelected(bool selected)
    {
        Color target = selected ? selectedColor : normalColor;

        foreach (var r in highlightRenderers)
        {
            if (r != null)
                r.material.color = target;
        }
    }
}