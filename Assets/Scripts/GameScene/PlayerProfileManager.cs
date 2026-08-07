using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerProfileManager : MonoBehaviour
{
    private string savePath; // file path for the save.

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "playerdata.json"); // save file location.
    }

    public PlayerData Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath); // read save text

            if (!string.IsNullOrEmpty(json))
            {
                PlayerData loadedData = JsonUtility.FromJson<PlayerData>(json); // turn text into data.
                if (loadedData != null)
                    return loadedData; 
            }
        }

        return new PlayerData(); // return empty data if no save exists.
    }

    public void Save(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true); // turn data into json.
        File.WriteAllText(savePath, json); // write save file.
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath); 
            Debug.Log("Save deleted."); 
        }
    }

    public void RequestTutorialReplay()
    {
        PlayerData data = Load();
        data.hasSeenTutorial = false;
        Save(data); // GameScene's TutorialController checks this flag on load and shows the tutorial from the start.
    }

    public void ResetPlantData()
    {
        DeleteSave(); 

        RoomPlant[] roomPlants = FindObjectsOfType<RoomPlant>(true); // find all room plants

        foreach (RoomPlant roomPlant in roomPlants)
        {
            if (roomPlant != null)
                roomPlant.ResetPlantState(); // reset each plant
        }

        Debug.Log("Plant inventory and timers reset."); 

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }
}