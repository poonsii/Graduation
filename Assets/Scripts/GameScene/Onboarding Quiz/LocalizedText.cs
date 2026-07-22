using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class LocalizedText
{
    private const string TableName = "Game Scene";

    public static string Get(string key, params object[] arguments)
    {
        LocalizedString localizedString = new LocalizedString(TableName, key);

        if (arguments != null && arguments.Length > 0)
            localizedString.Arguments = arguments;

        AsyncOperationHandle<string> handle = localizedString.GetLocalizedStringAsync();
        return handle.WaitForCompletion(); // force full resolution instead of trusting the convenience method to already be ready.
    }
}
