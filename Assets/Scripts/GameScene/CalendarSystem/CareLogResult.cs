using System.Collections.Generic;

// what actually happened when logging a care action - the points really awarded (after the
// 24-hour cooldown gate) and any badges newly earned as a result.
public class CareLogResult
{
    public int pointsAwarded;
    public List<string> newlyEarnedBadges = new List<string>();
}
