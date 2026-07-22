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

    public static string GetLabel(PotSoilType type)
    {
        string key;

        switch (type)
        {
            case PotSoilType.RegularPottingMix: key = "soil_regularpottingmix"; break;
            case PotSoilType.SucculentMix: key = "soil_succulentmix"; break;
            case PotSoilType.OrchidMix: key = "soil_orchidmix"; break;
            case PotSoilType.ChunkyAroidMix: key = "soil_chunkyaroidmix"; break;
            case PotSoilType.Water: key = "soil_water"; break;
            default: return type.ToString();
        }

        return LocalizedText.Get(key);
    }

    public static string GetLabel(HumidityLevel level)
    {
        string key;

        switch (level)
        {
            case HumidityLevel.Low: key = "humidity_low"; break;
            case HumidityLevel.Medium: key = "humidity_medium"; break;
            case HumidityLevel.High: key = "humidity_high"; break;
            default: return level.ToString();
        }

        return LocalizedText.Get(key);
    }
}
