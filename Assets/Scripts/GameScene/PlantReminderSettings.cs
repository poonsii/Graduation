using UnityEngine;

public class PlantReminderSettings : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject reminderCheckmark; 

    private const string RemindersEnabledKey = "PlantRemindersEnabled"; 

    public static bool RemindersEnabled
    {
        get => PlayerPrefs.GetInt(RemindersEnabledKey, 1) == 1; // load reminder setting, default is on.
        private set
        {
            PlayerPrefs.SetInt(RemindersEnabledKey, value ? 1 : 0); // save reminder setting.
            PlayerPrefs.Save();
        }
    }

    private void Awake()
    {
        RefreshCheckmark(); // update the checkmark on startup.
    }

    private void OnEnable()
    {
        RefreshCheckmark(); // update the checkmark whenever this object becomes active.
    }

    public void ToggleReminders()
    {
        RemindersEnabled = !RemindersEnabled; 
        RefreshAllReminderSettingsUI(); // update all reminder UI across the app.
    }

    public void SetRemindersEnabled(bool enabled)
    {
        RemindersEnabled = enabled; // set the reminder setting directly.
        RefreshAllReminderSettingsUI(); // update all reminder UI across the app.
    }

    public void RefreshCheckmark()
    {
        if (reminderCheckmark != null)
            reminderCheckmark.SetActive(RemindersEnabled); // show or hide the checkmark based on setting.
    }

    public static void RefreshAllReminderSettingsUI()
    {
        PlantReminderSettings[] allSettingsUIs =
            FindObjectsByType<PlantReminderSettings>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (PlantReminderSettings settingsUI in allSettingsUIs)
        {
            settingsUI.RefreshCheckmark(); // update every reminder settings UI in the scene.
        }

        if (!RemindersEnabled)
        {
            RoomPlant[] allPlants =
                FindObjectsByType<RoomPlant>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (RoomPlant plant in allPlants)
            {
                plant.HideReminderPopupFromSettings(); 
            }
        }
    }
}