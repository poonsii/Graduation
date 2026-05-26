using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections;

public class LanguageSwitcher : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;
    }

    public void NextLanguage()
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;

        if (locales == null || locales.Count == 0)
            return;

        int currentIndex = 0;

        for (int i = 0; i < locales.Count; i++)
        {
            if (LocalizationSettings.SelectedLocale == locales[i])
            {
                currentIndex = i;
                break;
            }
        }

        int nextIndex = (currentIndex + 1) % locales.Count;
        LocalizationSettings.SelectedLocale = locales[nextIndex];
    }

    public void SetLanguageByIndex(int index)
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;

        if (locales == null || index < 0 || index >= locales.Count)
            return;

        LocalizationSettings.SelectedLocale = locales[index];
    }

    public void SetLanguage(Locale locale)
    {
        if (locale != null)
            LocalizationSettings.SelectedLocale = locale;
    }
}