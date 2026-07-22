using UnityEngine;

public class PlantLocationSpot : MonoBehaviour
{
    public string displayName;
    public LightLocationType lightLocationType = LightLocationType.Unknown;

    [SerializeField] private Renderer[] highlightRenderers;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color badAdviceColor = Color.red;

    [Header("Marker Visual")]
    [SerializeField] private GameObject markerVisual; // the visible marker (e.g. a sphere) - separate from the collider, which stays active for raycasting.

    public LightLocationType GetLocationType() => lightLocationType;
    public string GetDisplayName() => string.IsNullOrEmpty(displayName) ? PlantAdviceText.GetLabel(lightLocationType) : displayName;
    public string GetSpotId() => gameObject.name; // unique per spot, even if two spots share the same LightLocationType.

    public void SetSelected(bool selected)
    {
        Color target = selected ? selectedColor : normalColor;
        ApplyColor(target);
    }

    public void SetAdviceColor(LightAdviceResult result)
    {
        Color target = result == LightAdviceResult.Good ? selectedColor : badAdviceColor;
        ApplyColor(target);
    }

    private void ApplyColor(Color target)
    {
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