using BattleShipEngine;

namespace BattleShipStrategies.Honza;

public class HonzaBoardCreationStrategy : IBoardCreationStrategy
{
    private Random random = new Random();
    private List<Int2> occupiedMap = new List<Int2>();
    private GameSetting setting { get; set; }

    public Int2[] GetBoatPositions(GameSetting s)
    {
        setting = s;
        List<Int2> boats = new List<Int2>();
        MarkBorders();

        for (int boatLength = setting.BoatCount.Length - 1; boatLength >= 0; boatLength--)
            for (int indexOfBoatType = 0; indexOfBoatType < setting.BoatCount[boatLength]; indexOfBoatType++)
            {
                List<Int2>[] positions = GetAllLegalPositions(boatLength);

                List<Int2> chosen = positions[random.Next(0, positions.Length)];
                boats.AddRange(chosen);
            }

        return boats.ToArray();
    }

    private List<Int2>[] GetAllLegalPositions(int boatLength)
    {
        //use map, return all positions of boats possible
        for (int x = 0; boatLength < (setting.Width - boatLength) + 1; x++)
            for (int y = 0; boatLength < (setting.Height - boatLength) + 1; x++)
            {
                // check horizontal

                // check vertical
            }
    }

    private void MarkBoatAndSurroundings(List<Int2> boat)
    {
        // Mark into `map`, places outside map can theoretically be marked
    }

    private void MarkBorders()
    {
        int xRightBorder = setting.Width;
        int xLeftBorder = -1;
        int yTopBorder = setting.Height;
        int yBotBorder = -1;

        var verticalWalls = Enumerable.Range(yBotBorder, yTopBorder);
        var horizontalWalls = Enumerable.Range(xLeftBorder, xRightBorder);

        foreach(var a in verticalWalls)
        {
            occupiedMap.Add(new Int2(a, xLeftBorder));
            occupiedMap.Add(new Int2(a, xRightBorder));
        }

        foreach (var b in horizontalWalls)
        {
            occupiedMap.Add(new Int2(b, yTopBorder));
            occupiedMap.Add(new Int2(b, yBotBorder));
        }

    }
}