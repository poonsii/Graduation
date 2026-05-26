using UnityEngine;

[CreateAssetMenu(menuName = "Furniture/Furniture Data")]
public class FurnitureData : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public GameObject prefab;

    public Vector2Int size = Vector2Int.one;
}