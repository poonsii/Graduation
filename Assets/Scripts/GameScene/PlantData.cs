using UnityEngine;

[CreateAssetMenu(menuName = "Plants/Plant Data")]
public class PlantData : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public GameObject cardPrefab;
    public GameObject roomItemPrefab;

    public GameObject healthyModel;
    public GameObject unhealthyModel;
}