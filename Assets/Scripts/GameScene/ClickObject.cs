using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickObject : MonoBehaviour
{
    private void OnMouseDown()
    {
        SceneManager.LoadScene("PlantCardScene");
    }
}
