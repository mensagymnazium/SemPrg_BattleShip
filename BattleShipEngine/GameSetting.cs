namespace BattleShipEngine;

/// <summary>
/// Represents the setting of a game
/// </summary>
/// <param name="Height">Height of the board</param>
/// <param name="Width">Width of the board</param>
/// <param name="BoatCount">The 0th element is the amount of <c>1 tile long boats</c>, the 1th element is the amount of <c>2 tile long boats</c>, ...</param>
public readonly record struct GameSetting(
    int Height,
    int Width,
    int[] BoatCount)
{
    public static bool AreSame(GameSetting s1, GameSetting s2)
    {
        if (s1.Width != s2.Width || s1.Height != s2.Height
                                 || s1.BoatCount.Length != s2.BoatCount.Length)
            return false;
        for (int i = 0; i < s1.BoatCount.Length; i++)
            if (s1.BoatCount[i] != s2.BoatCount[i])
                return false;
        return true;
    }
    
    /// <summary>
    /// Settings that can be found here <see href="https://www.codewars.com/kata/52bb6539a4cf1b12d90005b7"></see>
    /// </summary>
    public static GameSetting Default = new(
        10, 
        10, 
        new[] {4, 3, 2, 1});
    
    public static GameSetting Hasbro = new(
        10, 
        10, 
        new[] {0, 1, 2, 1, 1});
    
    public static GameSetting Large = new(
        12, 
        12, 
        new[] {0, 3, 2, 2, 1});
    
    public static GameSetting Small = new(
        5, 
        5, 
        new[] {2, 1, 1});
}
