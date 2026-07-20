using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private int cost = 500; 
    [SerializeField] private ConfirmPurchaseUI confirmUI; 
    [SerializeField] private GameObject toggleButton; 

    [Header("Buy Button")]
    [SerializeField] private Button buyButton; 
    [SerializeField] private TMP_Text buyButtonText; 
    [SerializeField] private Color greyedOutColor = new Color(0.5f, 0.5f, 0.5f, 1f); 

    [Header("Item Type")]
    [SerializeField] private ShopItemType itemType; // the type of item this is

    private const string NibblesKey = "NibblesPurchased"; // save key for the nibbles purchase.

    private void Start()
    {
        bool alreadyBought = false;

        if (itemType == ShopItemType.Shelf)
            alreadyBought = ShelfSwapper.Instance != null && ShelfSwapper.Instance.IsPurchased(); // check if the shelf was already bought.

        if (itemType == ShopItemType.Nibbles)
            alreadyBought = PlayerPrefs.GetInt(NibblesKey, 0) == 1; // check if nibbles was already bought.

        if (alreadyBought)
            SetPurchased(); 
    }

    public void OnBuyButtonClicked()
    {
        confirmUI.Show(cost, OnConfirmed, OnCancelled); // show the confirmation panel.
    }

    private void OnConfirmed()
    {
        if (!PointsManager.Instance.TrySpendPoints(cost))
        {
            Debug.Log("Not enough points.");
            return; 
        }

        if (itemType == ShopItemType.Shelf)
        {
            ShelfSwapper.Instance.ActivateNewShelf(); // switch to the new shelf.

            if (toggleButton != null)
                toggleButton.SetActive(true); // show the toggle button.
        }

        if (itemType == ShopItemType.Nibbles)
        {
            PlayerPrefs.SetInt(NibblesKey, 1);
            PlayerPrefs.Save(); // save the nibbles purchase.
            NibblesEvolution.Instance.Evolve(); 
        }

        SetPurchased();
        Debug.Log(itemType + " purchased!");
    }

    private void OnCancelled()
    {
        Debug.Log("Purchase cancelled.");
    }

    private void SetPurchased()
    {
        if (buyButton != null)
            buyButton.interactable = false; // disable the buy button.

        if (buyButtonText != null)
            buyButtonText.text = "Purchased"; // update the text.
    }

    public void ResetPurchase()
    {
        if (buyButton != null)
            buyButton.interactable = true; // re-enable the buy button.

        if (buyButtonText != null)
            buyButtonText.text = cost + " points"; // restore the original text.
    }
}

public enum ShopItemType
{
    Shelf,
    Nibbles
}