using UnityEngine;

public class OwnedPlantCardUI : MonoBehaviour
{
    [SerializeField] private string plantId;
    [SerializeField] private GameObject cardRoot;

    public string GetPlantId() // get the plant id.
    {
        return plantId;
    }

    public void SetOwned(bool owned) // show if it is owned.
    {
        if (cardRoot != null)
            cardRoot.SetActive(owned);
        else
            gameObject.SetActive(owned);
    }
}