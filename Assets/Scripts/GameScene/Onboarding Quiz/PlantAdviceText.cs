public static class PlantAdviceText
{
    public static string GetLabel(LightAdviceResult result) => GetLabel((int)result);

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

    public static string GetLabel(LightLocationType type)
    {
        string key;

        switch (type)
        {
            case LightLocationType.DirectSun: key = "light_directsun"; break;
            case LightLocationType.BrightIndirect: key = "light_brightindirect"; break;
            case LightLocationType.MediumLight: key = "light_mediumlight"; break;
            case LightLocationType.LowLight: key = "light_lowlight"; break;
            default: return type.ToString();
        }

        return LocalizedText.Get(key);
    }
}
