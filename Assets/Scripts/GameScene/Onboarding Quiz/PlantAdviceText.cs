public static class PlantAdviceText
{
    public static string GetLabel(LightAdviceResult result) => GetLabel((int)result);
    public static string GetLabel(PotSoilAdviceResult result) => GetLabel((int)result);
    public static string GetLabel(HumidityAdviceResult result) => GetLabel((int)result);

    private static string GetLabel(int adviceValue)
    {
        string key;

        switch (adviceValue)
        {
            case 1: key = "advice_good"; break;
            case 2: key = "advice_warning"; break;
            case 3: key = "advice_bad"; break;
            default: key = "advice_unknown"; break;
        }

        return LocalizedText.Get(key);
    }
}
