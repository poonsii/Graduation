public static class PlantSeasonAdvisor
{
    public static bool IsHumiditySeason(PlantData plant, int month)
    {
        return plant != null && plant.humiditySeasonMonths != null && plant.humiditySeasonMonths.Contains(month);
    }

    public static bool IsFertilizingSeason(PlantData plant, int month)
    {
        return plant != null && plant.fertilizingSeasonMonths != null && plant.fertilizingSeasonMonths.Contains(month);
    }
}
