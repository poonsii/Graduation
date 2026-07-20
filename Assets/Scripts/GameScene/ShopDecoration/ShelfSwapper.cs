using UnityEngine;

public class ShelfSwapper : MonoBehaviour
{
    public static ShelfSwapper Instance; 

    [SerializeField] private GameObject defaultShelf; 
    [SerializeField] private GameObject newShelf; 
    [SerializeField] private GameObject toggleButton; 
    [SerializeField] private GameObject resetButton; 

    [Header("Shop Items")]
    [SerializeField] private ShopItem shelfShopItem; 
    [SerializeField] private ShopItem nibblesShopItem; 

    private const string ShelfKey = "NewShelfActive"; 
    private bool newShelfActive = false; 

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        newShelfActive = PlayerPrefs.GetInt(ShelfKey, 0) == 1; // load saved shelf state.
        ApplyShelfState();

        // show toggle button if shelf was already purchased.
        if (toggleButton != null)
            toggleButton.SetActive(newShelfActive);
    }

    public void ActivateNewShelf()
    {
        newShelfActive = true;
        Save();
        ApplyShelfState();
        Debug.Log("New shelf activated and saved.");
    }

    public void ToggleShelf()
    {
        newShelfActive = !newShelfActive; // flip between the two shelves.
        Save();
        ApplyShelfState();
        Debug.Log("Shelf toggled to: " + (newShelfActive ? "New" : "Default"));
    }

    private void ApplyShelfState()
    {
        defaultShelf.SetActive(!newShelfActive); // show or hide the default shelf.
        newShelf.SetActive(newShelfActive); // show or hide the new shelf.
    }

    private void Save() // save the current shelf state.
    {
        PlayerPrefs.SetInt(ShelfKey, newShelfActive ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool IsPurchased()
    {
        return PlayerPrefs.GetInt(ShelfKey, 0) == 1; // return whether the shelf has been purchased.
    }

    public void ResetPurchase()
    {
        // reset shelf.
        newShelfActive = false;
        PlayerPrefs.DeleteKey(ShelfKey); 
        ApplyShelfState();

        if (toggleButton != null)
            toggleButton.SetActive(false); 

        if (shelfShopItem != null)
            shelfShopItem.ResetPurchase(); 

        // reset nibbles.
        PlayerPrefs.DeleteKey("NibblesPurchased");
        if (NibblesEvolution.Instance != null)
            NibblesEvolution.Instance.ResetEvolution(); // reset nibbles to caterpillar.

        if (nibblesShopItem != null)
            nibblesShopItem.ResetPurchase(); 

        // save once at the end.
        PlayerPrefs.Save();

        
        if (resetButton != null)
            resetButton.SetActive(true);

        Debug.Log("All purchases reset.");
    }
}