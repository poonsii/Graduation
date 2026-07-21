public static class PlantAdviceText
{
    public static string GetLabel(LightAdviceResult result) => GetLabel((int)result);
    public static string GetLabel(PotSoilAdviceResult result) => GetLabel((int)result);
    public static string GetLabel(HumidityAdviceResult result) => GetLabel((int)result);

    private static string GetLabel(int adviceValue)
    {
        switch (adviceValue)
        {
            case 1: return "Good fit";
            case 2: return "Not ideal";
            case 3: return "Poor fit";
            default: return "Not set";
        }
    }
}
