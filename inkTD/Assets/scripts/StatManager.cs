using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A list of stats present within the game.
/// </summary>
public enum Stats
{
    None = 0,
    CreaturesKilled = 1,
    CreaturesSpawned = 2, //Done
    TowersCreated = 3, //Done
    ProjectilesShots = 4, //Done
    InkSpent = 5, //Done
    InkAccumulated = 6, //Done
    Time = 7,
    PathLength = 8,
    TowersUpgraded = 9,
    TowersSold = 10,

}

public static class StatManager
{

    private static Dictionary<int, Dictionary<Stats, double>> stats = new Dictionary<int, Dictionary<Stats, double>>();


    private static void CheckKeyValidity(int playerID)
    {
        if (!stats.ContainsKey(playerID))
        {
            stats.Add(playerID, new Dictionary<Stats, double>());
        }
    }

    /// <summary>
    /// Adds to a specific stat for a player.
    /// </summary>
    /// <param name="playerID">The player's ID.</param>
    /// <param name="stat">The stat being modified.</param>
    /// <param name="value">The value to add to the stat.</param>
    public static void AddStat(int playerID, Stats stat, double value)
    {
        CheckKeyValidity(playerID);

        if (!stats[playerID].ContainsKey(stat))
            stats[playerID].Add(stat, 0d);

        stats[playerID][stat] += value;
    }

    /// <summary>
    /// Adds to a specific stat for a player.
    /// </summary>
    /// <param name="playerID">The player's ID.</param>
    /// <param name="stat">The stat being modified.</param>
    /// <param name="value">The value to add to the stat.</param>
    public static void SetStat(int playerID, Stats stat, double value)
    {
        CheckKeyValidity(playerID);
        stats[playerID][stat] = value;
    }

    /// <summary>
    /// Gets the specific stat.
    /// </summary>
    /// <param name="playerID">The player's ID.</param>
    /// <param name="stat">The stat will be returned..</param>
    /// <returns></returns>
    public static double GetStat(int playerID, Stats stat)
    {
        CheckKeyValidity(playerID);
        return stats[playerID][stat];
    }
}
