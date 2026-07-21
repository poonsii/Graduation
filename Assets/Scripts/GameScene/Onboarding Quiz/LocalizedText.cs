using UnityEngine.Localization;

public static class LocalizedText
{
    private const string TableName = "Game Scene";

    public static string Get(string key, params object[] arguments)
    {
        LocalizedString localizedString = new LocalizedString(TableName, key);

        if (arguments != null && arguments.Length > 0)
            localizedString.Arguments = arguments;

        return localizedString.GetLocalizedString();
    }
}
