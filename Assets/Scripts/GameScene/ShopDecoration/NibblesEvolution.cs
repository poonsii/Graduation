using UnityEngine;

public class NibblesEvolution : MonoBehaviour
{
    public static NibblesEvolution Instance; 

    [SerializeField] private int evolutionCost = 1000; 
    [SerializeField] private GameObject caterpillarModel; 
    [SerializeField] private GameObject butterflyModel; 

    private const string NibblesKey = "NibblesPurchased"; 
    private bool evolved = false; 

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        evolved = PlayerPrefs.GetInt(NibblesKey, 0) == 1; // load saved evolution state.
        ApplyVisuals();
    }

    public void Evolve() // switching to butterfly
    {
        if (evolved) return; 
        evolved = true;
        ApplyVisuals(); 
        Debug.Log("Nibbles turned into a butterfly!");
    }

    public void ResetEvolution() // switch back to caterpillar.
    {
        evolved = false;
        PlayerPrefs.DeleteKey(NibblesKey); 
        PlayerPrefs.Save();
        ApplyVisuals(); 
    }

    private void ApplyVisuals()
    {
        if (caterpillarModel != null) caterpillarModel.SetActive(!evolved); // show caterpillar if not evolved.
        if (butterflyModel != null) butterflyModel.SetActive(evolved); // show butterfly if evolved.
    }
}