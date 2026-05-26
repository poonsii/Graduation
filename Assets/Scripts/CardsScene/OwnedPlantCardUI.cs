using UnityEngine;

public class OwnedPlantCardUI : MonoBehaviour
{
    [SerializeField] private string plantId;
    [SerializeField] private GameObject cardRoot;

    public string GetPlantId()
    {
        return plantId;
    }

    public void SetOwned(bool owned)
    {
        if (cardRoot != null)
            cardRoot.SetActive(owned);
        else
            gameObject.SetActive(owned);
    }
}