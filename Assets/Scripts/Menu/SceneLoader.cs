using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void LoadOptions()
    {
        SceneManager.LoadScene("Options");
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void LoadPlantCardScene()
    {
        SceneManager.LoadScene("PlantCardScene");
    }

    public void LoadCardsScene()
    {
        SceneManager.LoadScene("CardsScene");
    }
}
