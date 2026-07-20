using System;

public static class PlantLocationAdvisor
{
    public static LightAdviceResult GetAdvice(LightLocationType requiredLight, LightLocationType selectedLight)
    {
        if (requiredLight == LightLocationType.Unknown || selectedLight == LightLocationType.Unknown)
            return LightAdviceResult.Unknown;

        int difference = Math.Abs((int)requiredLight - (int)selectedLight);

        if (difference == 0)
            return LightAdviceResult.Good;

        if (difference == 1)
            return LightAdviceResult.Warning;

        return LightAdviceResult.Bad;
    }
}