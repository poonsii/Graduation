using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerProfileManager : MonoBehaviour
{
    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "playerdata.json");
    }

    public PlayerData Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);

            if (!string.IsNullOrEmpty(json))
            {
                PlayerData loadedData = JsonUtility.FromJson<PlayerData>(json);
                if (loadedData != null)
                    return loadedData;
            }
        }

        return new PlayerData();
    }

    public void Save(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save deleted.");
        }
    }

    public void ResetPlantData()
    {
        DeleteSave();

        RoomPlant[] roomPlants = FindObjectsOfType<RoomPlant>(true);

        foreach (RoomPlant roomPlant in roomPlants)
        {
            if (roomPlant != null)
                roomPlant.ResetPlantState();
        }

        Debug.Log("Plant inventory and timers reset.");

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}