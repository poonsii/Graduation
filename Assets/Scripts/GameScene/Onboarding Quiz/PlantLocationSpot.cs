using UnityEngine;

public class PlantLocationSpot : MonoBehaviour
{
    public string displayName;
    public LightLocationType lightLocationType = LightLocationType.Unknown;

    [SerializeField] private Renderer[] highlightRenderers;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.green;

    [Header("Marker Visual")]
    [SerializeField] private GameObject markerVisual; // the visible marker (e.g. a sphere) - separate from the collider, which stays active for raycasting.

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

    public void SetMarkerVisible(bool visible)
    {
        if (markerVisual != null)
            markerVisual.SetActive(visible);
    }
}