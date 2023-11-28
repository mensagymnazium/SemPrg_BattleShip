using BattleShipEngine;
using System.Drawing;

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
                List<List<Int2>> positions = GetAllLegalPositions(boatLength);

                List<Int2> chosen = positions[random.Next(positions.Count)];
                MarkBoatAndSurroundings(chosen);
                boats.AddRange(chosen);
            }

        return boats.ToArray();
    }

    private List<List<Int2>> GetAllLegalPositions(int boatLength)
    {
        List<List<Int2>> positions = new List<List<Int2>>();
        //use map, return all positions of boats possible
        for (int x = 0; boatLength < (setting.Width - boatLength) + 1; x++)
            for (int y = 0; boatLength < (setting.Height - boatLength) + 1; x++)
            {
                // check horizontal
                List<Int2> horizontal = new List<Int2>();
                bool hOK = true;
                for (int i = 0; i < boatLength; i++)
                {
                    var s = new Int2(x + i, y);
                    if (!occupiedMap.Contains(s)) horizontal.Add(s);
                    else
                    {
                        hOK = true;
                        break;
                    }
                }

                // check vertical
                List<Int2> vertical = new List<Int2>();
                bool vOK = true;
                for (int i = 0; i < boatLength; i++)
                {
                    var s = new Int2(x, y + 1);
                    if (!occupiedMap.Contains(s)) vertical.Add(s);
                    else
                    {
                        vOK = true;
                        break;
                    }
                }

                if (hOK)
                {
                    positions.Add(horizontal);
                }
                if (vOK)
                {
                    positions.Add(vertical);
                }
            }
        return positions;
    }

    private void MarkBoatAndSurroundings(List<Int2> boat)
    {
        foreach (Int2 p in boat)
        {
            occupiedMap.Add(p + new Int2(0, 0));
            occupiedMap.Add(p + new Int2(0, 1));
            occupiedMap.Add(p + new Int2(0, -1));
            occupiedMap.Add(p + new Int2(1, 0));
            occupiedMap.Add(p + new Int2(1, 1));
            occupiedMap.Add(p + new Int2(1, -1));
            occupiedMap.Add(p + new Int2(-1, 0));
            occupiedMap.Add(p + new Int2(-1, 1));
            occupiedMap.Add(p + new Int2(-1, -1));
        }

        occupiedMap = occupiedMap.Distinct().ToList();
    }

    private void MarkBorders()
    {
        int xRightBorder = setting.Width;
        int xLeftBorder = -1;
        int yTopBorder = setting.Height;
        int yBotBorder = -1;

        var verticalWalls = Enumerable.Range(yBotBorder, yTopBorder);
        var horizontalWalls = Enumerable.Range(xLeftBorder, xRightBorder);

        foreach (var a in verticalWalls)
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