using BattleShipEngine;

namespace BattleShipStrategies.Slavek.AI;

public readonly record struct CoefficientMap(GameSetting Settings, double[,] Coefficients)
{
    public static CoefficientMap operator +(CoefficientMap map1, CoefficientMap map2)
    {
        if (map1.Settings.Width != map2.Settings.Width
            || map1.Settings.Height != map2.Settings.Height)
            throw new Exception("Cannot add two maps with different Width/Height.");
        for (int i = 0; i < map1.Settings.Width; i++)
        for (int j = 0; j < map1.Settings.Height; j++)
            map1.Coefficients[i, j] *= map2.Coefficients[i, j];
        return map1;
    }

    public CoefficientMap CloneCoefficients()
    {
        return this with {Coefficients = Coefficients.Clone() as double[,]};
    }

    public override string ToString()
    {
        string result = "{";
        for (int i = 0; i < Settings.Width; i++)
        {
            result += '{';
            for (int j = 0; j < Settings.Height; j++)
            {
                result += Coefficients[i, j].ToString();
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