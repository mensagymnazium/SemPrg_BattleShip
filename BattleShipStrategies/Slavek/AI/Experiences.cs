using BattleShipEngine;

namespace BattleShipStrategies.Slavek.AI;

public partial record struct Experiences(string StrategyName, GameSetting Settings,
    CoefficientMap InitialCoefficients, Dictionary<(Int2, SlavekTile), CoefficientMap?> Changes)
{
    public static Experiences Blank(string name, GameSetting setting)
    {
        double[,] coefficients = new double[setting.Width, setting.Height];
        Dictionary<(Int2, SlavekTile), CoefficientMap?> changes = new();
        for (int i = 0; i < setting.Width; i++)
            for (int j = 0; j < setting.Height; j++)
            {
                changes[(new Int2(i, j), SlavekTile.Boat)] = null;
                changes[(new Int2(i, j), SlavekTile.Water)] = null;
                coefficients[i, j] = 0.2;
            }
        return new Experiences(name, setting, 
            new CoefficientMap(setting, coefficients, 0), changes);
    }
    
    public void AddChange(Int2 position, SlavekTile shot, CoefficientMap? changeMap)
    {
        Changes.Add((position, shot), changeMap);
    }

    public static Experiences operator +(Experiences experiences, SlavekTile[,] board)
    {
        if (experiences.Settings.Width != board.GetLength(0)
            || experiences.Settings.Height != board.GetLength(1))
            throw new Exception("Cannot add a map with different Width/Height.");

        CoefficientMap newInitialCoefficients = experiences.InitialCoefficients + board;
        CoefficientMap singleBoard = new CoefficientMap(experiences.Settings,
            new double[experiences.Settings.Width, experiences.Settings.Height], 1);
            
        for (int i = 0; i < experiences.Settings.Width; i++)
        for (int j = 0; j < experiences.Settings.Height; j++)
        {
            if (board[i, j] == SlavekTile.Unknown)
                board[i, j] = SlavekTile.Water;
            if (board[i, j] == SlavekTile.DamagedBoat)
            {
                board[i, j] = SlavekTile.Boat;
                singleBoard.Coefficients[i, j] = 1;
            }
            else
                singleBoard.Coefficients[i, j] = 0;
        }
        singleBoard /= newInitialCoefficients;

        for (int i = 0; i < experiences.Settings.Width; i++)
        for (int j = 0; j < experiences.Settings.Height; j++)
        {
            if (experiences.Changes[(new Int2(i, j), board[i, j])] is not null)
                experiences.Changes[(new Int2(i, j), board[i, j])] =
                    ((CoefficientMap)experiences.Changes[(new Int2(i, j), board[i, j])]
                    * experiences.InitialCoefficients + board) / newInitialCoefficients;
            else
                experiences.Changes[(new Int2(i, j), board[i, j])] = singleBoard;
        }

        experiences.InitialCoefficients = newInitialCoefficients;
        
        return experiences;
    }
}
