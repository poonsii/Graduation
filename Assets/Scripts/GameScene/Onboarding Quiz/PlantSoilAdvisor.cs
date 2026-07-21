public static class PlantSoilAdvisor
{
    public static PotSoilAdviceResult GetAdvice(PotSoilType recommendedSoil, PotSoilType selectedSoil)
    {
        if (recommendedSoil == PotSoilType.Unknown || selectedSoil == PotSoilType.Unknown)
            return PotSoilAdviceResult.Unknown;

        if (recommendedSoil == selectedSoil)
            return PotSoilAdviceResult.Good;

        bool recommendedIsWater = recommendedSoil == PotSoilType.Water;
        bool selectedIsWater = selectedSoil == PotSoilType.Water;

        if (recommendedIsWater != selectedIsWater)
            return PotSoilAdviceResult.Bad;

        return PotSoilAdviceResult.Warning;
    }
}
