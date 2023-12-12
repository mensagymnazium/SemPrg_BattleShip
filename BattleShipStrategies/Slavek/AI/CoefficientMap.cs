using System.Globalization;
using BattleShipEngine;

namespace BattleShipStrategies.Slavek.AI;

public record struct CoefficientMap(GameSetting Settings, double[,] Coefficients,
    uint HowSure=0)
{
    public static CoefficientMap operator *(CoefficientMap map1, CoefficientMap map2)
    {
        if (map1.Settings.Width != map2.Settings.Width
            || map1.Settings.Height != map2.Settings.Height)
            throw new Exception("Cannot multiply two maps with different Width/Height.");
        for (int i = 0; i < map1.Settings.Width; i++)
        for (int j = 0; j < map1.Settings.Height; j++)
            map1.Coefficients[i, j] *= map2.Coefficients[i, j];
        if (map1.HowSure > map2.HowSure)
            map1.HowSure = map2.HowSure;
        return map1;
    }

    public static CoefficientMap operator /(CoefficientMap map1, CoefficientMap map2)
    {
        if (map1.Settings.Width != map2.Settings.Width
            || map1.Settings.Height != map2.Settings.Height)
            throw new Exception("Cannot divide two maps with different Width/Height.");
        for (int i = 0; i < map1.Settings.Width; i++)
        for (int j = 0; j < map1.Settings.Height; j++)
            map1.Coefficients[i, j] /= map2.Coefficients[i, j];
        if (map1.HowSure > map2.HowSure)
            map1.HowSure = map2.HowSure;
        return map1;
    }

    public static CoefficientMap operator +(CoefficientMap map, SlavekTile[,] board)
    {
        if (map.Settings.Width != board.GetLength(0)
            || map.Settings.Height != board.GetLength(1))
            throw new Exception("Cannot add two maps with different Width/Height.");
        for (int i = 0; i < map.Settings.Width; i++)
        for (int j = 0; j < map.Settings.Height; j++)
        {
            if (board[i, j] == SlavekTile.Boat)
                map.Coefficients[i, j] = (map.Coefficients[i, j] * map.HowSure + 1)
                                         / (map.HowSure + 1);
            else
                map.Coefficients[i, j] = map.Coefficients[i, j] * map.HowSure
                                         / (map.HowSure + 1);
        }
        map.HowSure++;
        return map;
    }

    public CoefficientMap CloneCoefficients()
    {
        return this with {Coefficients = (double[,]) Coefficients.Clone()};
    }

    public override string ToString()
    {
        string result = "{";
        for (int i = 0; i < Settings.Width; i++)
        {
            result += '{';
            for (int j = 0; j < Settings.Height; j++)
            {
                result += Coefficients[i, j].ToString("G", CultureInfo.InvariantCulture);
                if (j < Settings.Height - 1)
                    result += ',';
            }
            result += '}';
            if (i < Settings.Width - 1)
                result += ",\n";
        }
        result += '}';
        return result;
    }
}