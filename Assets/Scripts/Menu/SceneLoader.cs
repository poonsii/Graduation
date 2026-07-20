using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadGameScene() 
    {
        SceneManager.LoadScene("GameScene");
    }

    public void LoadOptions() //options opens.
    {
        SceneManager.LoadScene("Options");
    }

    public void LoadMenuScene() //menu opens.
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void LoadPlantCardScene() //not used anymore.
    {
        SceneManager.LoadScene("PlantCardScene");
    }

    public void LoadCardsScene() //not used anymore.
    {
        SceneManager.LoadScene("CardsScene");
    }
}
