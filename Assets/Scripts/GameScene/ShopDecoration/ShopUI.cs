using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;

   
    public void OpenShop() // open shop panel.
    {
        shopPanel.SetActive(true);
    }

    
    public void CloseShop() // close shop panel.
    {
        shopPanel.SetActive(false);
    }
}