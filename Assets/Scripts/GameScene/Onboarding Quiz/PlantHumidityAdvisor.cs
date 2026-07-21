using System;

public static class PlantHumidityAdvisor
{
    public static HumidityAdviceResult GetAdvice(HumidityLevel recommendedHumidity, HumidityLevel selectedHumidity)
    {
        if (recommendedHumidity == HumidityLevel.Unknown || selectedHumidity == HumidityLevel.Unknown)
            return HumidityAdviceResult.Unknown;

        int difference = Math.Abs((int)recommendedHumidity - (int)selectedHumidity);

        if (difference == 0)
            return HumidityAdviceResult.Good;

        if (difference == 1)
            return HumidityAdviceResult.Warning;

        return HumidityAdviceResult.Bad;
    }
}
