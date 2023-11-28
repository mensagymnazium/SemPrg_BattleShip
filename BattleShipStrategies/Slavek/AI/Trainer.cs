using System.Text;
using BattleShipEngine;

namespace BattleShipStrategies.Slavek.AI;

public class Trainer
{
    public static Experiences Train(string name, IBoardCreationStrategy strategy,
        GameSetting settings, int boardCount)
    {
        SlavekTile[,,] boards = new SlavekTile[settings.Width, settings.Height, boardCount];
        for (int i = 0; i < boardCount; i++)
        {
            Int2[] boats = strategy.GetBoatPositions(settings);
            for (int x = 0; x < settings.Width; x++)
            for (int y = 0; y < settings.Height; y++)
            {
                if (boats.Contains(new Int2(x, y)))
                    boards[x, y, i] = SlavekTile.Boat;
                else
                    boards[x, y, i] = SlavekTile.Water;
            }
        }

        double[,] probabilities = new double[settings.Width, settings.Height];
        int[,] a = new int[3,2] { {0, 1}, {1, 2}, {0, 0} };
        for (int x = 0; x < settings.Width; x++)
        for (int y = 0; y < settings.Height; y++)
        {
            int probability = 0;
            for (int i = 0; i < boardCount; i++)
                if (boards[x, y, i] == SlavekTile.Boat)
                    probability++;
            probabilities[x,y] = probability / (double) boardCount;
        }
        
        Experiences experiences = new Experiences(name, settings,
            new CoefficientMap(settings, probabilities),
            new Dictionary<(Int2, SlavekTile), CoefficientMap?>());
        
        for (int w = 0; w < settings.Width; w++)
        for (int h = 0; h < settings.Height; h++)
            foreach (SlavekTile s in new[] { SlavekTile.Boat, SlavekTile.Water })
            {
                probabilities = new double[settings.Width, settings.Height];
                int count = 0;
                for (int i = 0; i < boardCount; i++)
                    if (boards[w, h, i] == s)
                        count++;
                if (count > 5)
                {
                    for (int x = 0; x < settings.Width; x++)
                    for (int y = 0; y < settings.Height; y++)
                    {
                        if (experiences.InitialCoefficients.Coefficients[x, y] != 0)
                        {
                            int probability = 0;
                            for (int i = 0; i < boardCount; i++)
                                if (boards[x, y, i] == SlavekTile.Boat && boards[w, h, i] == s)
                                    probability++;
                            probabilities[x, y] = probability / (double)count /
                                                  experiences.InitialCoefficients.Coefficients[x, y];
                        }
                        else
                            probabilities[x, y] = 0;
                    }

                    experiences.AddChange(new Int2(w, h), s,
                        new CoefficientMap(settings, probabilities));
                }
                else
                    experiences.AddChange(new Int2(w, h), s, null);
            }
        
        return experiences;
    }

    public static void WriteToData(string name, Experiences experiences)
    {
        FileStream stream = File.Open("BattleShipStrategies/Slavek/AI/Data/" + name + ".cs",
            FileMode.Create, FileAccess.Write);
        UTF8Encoding utf = new UTF8Encoding(true);
        
        stream.Write(utf.GetBytes(
            "using BattleShipEngine;\n\n" +
            "namespace BattleShipStrategies.Slavek.AI;\n\n" +
            "public readonly partial record struct Experiences\n" +
            "{\n" +
            "    public static Experiences " + name + "()\n" +
            "    {\n"));
        
        string s = "        GameSetting s = new GameSetting("
                   + experiences.Settings.Width + ','
                   + experiences.Settings.Height + ", new int[] {";
        for (int i = 0; i < experiences.Settings.BoatCount.Length; i++)
        {
            s += experiences.Settings.BoatCount[i];
            if (i < experiences.Settings.BoatCount.Length - 1)
                s += ',';
        }
        s += "});\n";
        stream.Write(utf.GetBytes(s));
        
        stream.Write(utf.GetBytes(
            "        Experiences e = new Experiences(\"" + name + "\", s,\n" +
            "            new CoefficientMap(s, new double[,] \n"
            + experiences.InitialCoefficients.ToString() + "),\n" +
            "            new Dictionary<(Int2,SlavekTile),CoefficientMap?>());\n"));

        foreach (var change in experiences.Changes)
        {
            if (change.Value is null)
                stream.Write(utf.GetBytes(
                    "        e.AddChange(new Int2(" 
                    + change.Key.Item1.X + "," + change.Key.Item1.Y + ")," +
                    "SlavekTile." + change.Key.Item2.ToString() + 
                    ", null);\n"));
            else
                stream.Write(utf.GetBytes(
                    "        e.AddChange(new Int2(" 
                    + change.Key.Item1.X + "," + change.Key.Item1.Y + ")," +
                    "SlavekTile." + change.Key.Item2.ToString() + 
                    ", new CoefficientMap(s, new double[,] \n" 
                    + change.Value.ToString() + "));\n"));
        }
        
        stream.Write(utf.GetBytes(
            "        return e;\n" +
            "    }\n" +
            "}\n"));
        stream.Close();
    }

    public static void TrainAndWrite(string name, IBoardCreationStrategy strategy,
        GameSetting settings, int boardCount)
    {
        WriteToData(name, Train(name, strategy, settings, boardCount));
    }
}