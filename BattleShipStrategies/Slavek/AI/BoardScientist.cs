using BattleShipEngine;
using System.Text;

namespace BattleShipStrategies.Slavek.AI;

public static class BoardScientist
{
    public static PreparedMap StableNemesis(GameSetting setting, IGameStrategy strategy, uint tries=10000)
    {
        IBoardCreationStrategy boardStrategy = new SmartRandomBoardCreationStrategy();
        int boatTileCount = 0;
        for (int i = 0; i < setting.BoatCount.Length; i++)
            boatTileCount += setting.BoatCount[i] * (i + 1);
        
        Int2[] bestBoats = new Int2[boatTileCount];
        int bestTurns = 0;
        for (uint i = 0; i < tries; i++)
        {
            Int2[] newBoats = boardStrategy.GetBoatPositions(setting);
            Game game = new Game(newBoats, setting);
            int newTurns = game.SimulateGame(strategy);
            if (newTurns > bestTurns)
            {
                bestTurns = newTurns;
                bestBoats = newBoats;
            }
        }
        Console.WriteLine(bestTurns);
        return new PreparedMap(setting, bestBoats);
    }
    
    public static PreparedMap UnstableNemesis(GameSetting setting, IGameStrategy strategy,
        uint tries=10000, uint precision=20)
    {
        IBoardCreationStrategy boardStrategy = new SmartRandomBoardCreationStrategy();
        int boatTileCount = 0;
        for (int i = 0; i < setting.BoatCount.Length; i++)
            boatTileCount += setting.BoatCount[i] * (i + 1);
        
        Int2[] bestBoats = new Int2[boatTileCount];
        int bestTurns = 0;
        for (uint i = 0; i < tries; i++)
        {
            Int2[] newBoats = boardStrategy.GetBoatPositions(setting);
            Game game = new Game(newBoats, setting);
            int newTurns = 0;
            for (uint j = 0; j < precision; j++)
                newTurns += game.SimulateGame(strategy);
            if (newTurns > bestTurns)
            {
                bestTurns = newTurns;
                bestBoats = newBoats;
            }
        }
        Console.WriteLine(bestTurns/precision);
        return new PreparedMap(setting, bestBoats);
    }

    public static PreparedMap StableSmart(GameSetting setting, Dictionary<IGameStrategy, double> enemies,
        Dictionary<IGameStrategy, double> allies, uint tries=10000)
    {
        IBoardCreationStrategy boardStrategy = new SmartRandomBoardCreationStrategy();
        int boatTileCount = 0;
        for (int i = 0; i < setting.BoatCount.Length; i++)
            boatTileCount += setting.BoatCount[i] * (i + 1);
        
        Int2[] bestBoats = new Int2[boatTileCount];
        double bestScore = 0;
        for (uint i = 0; i < tries; i++)
        {
            Int2[] newBoats = boardStrategy.GetBoatPositions(setting);
            Game game = new Game(newBoats, setting);
            double newScore = 0;
            foreach (IGameStrategy strategy in enemies.Keys)
                newScore += game.SimulateGame(strategy) * enemies[strategy];
            foreach (IGameStrategy strategy in allies.Keys)
                newScore -= game.SimulateGame(strategy) * allies[strategy];
            if (newScore > bestScore)
            {
                bestScore = newScore;
                bestBoats = newBoats;
            }
        }
        Console.WriteLine(bestScore);
        return new PreparedMap(setting, bestBoats);
    }
    
    public static PreparedMap UnstableSmart(GameSetting setting, Dictionary<IGameStrategy,
            (double coefficient, bool stable)> enemies,
        Dictionary<IGameStrategy, (double coefficient, bool stable)> allies, uint tries=10000, uint precision=20)
    {
        IBoardCreationStrategy boardStrategy = new SmartRandomBoardCreationStrategy();
        int boatTileCount = 0;
        for (int i = 0; i < setting.BoatCount.Length; i++)
            boatTileCount += setting.BoatCount[i] * (i + 1);
        
        Int2[] bestBoats = new Int2[boatTileCount];
        double bestScore = 0;
        for (uint i = 0; i < tries; i++)
        {
            Int2[] newBoats = boardStrategy.GetBoatPositions(setting);
            Game game = new Game(newBoats, setting);
            double newScore = 0;
            
            foreach (IGameStrategy strategy in enemies.Keys)
                for (uint j = 0; j < (enemies[strategy].stable ? 1 : precision); j++)
                    newScore += game.SimulateGame(strategy) * enemies[strategy].coefficient *
                                (enemies[strategy].stable ? precision : 1);
            foreach (IGameStrategy strategy in allies.Keys)
                for (uint j = 0; j < (allies[strategy].stable ? 1 : precision); j++)
                    newScore -= game.SimulateGame(strategy) * allies[strategy].coefficient * 
                                (allies[strategy].stable ? precision : 1);

            if (newScore > bestScore)
            {
                bestScore = newScore;
                bestBoats = newBoats;
            }
        }
        Console.WriteLine(bestScore);
        return new PreparedMap(setting, bestBoats);
    }

    public static void WritePrepared(string name, PreparedMap map)
    {
        FileStream stream = File.Open("BattleShipStrategies/Slavek/AI/Data/" + name + ".cs",
            FileMode.Create, FileAccess.Write);
        UTF8Encoding utf = new UTF8Encoding(true);
        
        stream.Write(utf.GetBytes(
            "using BattleShipEngine;\n\n" +
            "namespace BattleShipStrategies.Slavek.AI;\n\n" +
            "public partial record struct PreparedMap\n" +
            "{\n" +
            "    public static PreparedMap " + name + "()\n" +
            "    {\n"));
        
        string s = "        GameSetting s = new GameSetting("
                   + map.Settings.Width + ','
                   + map.Settings.Height + ", new int[] {";
        for (int i = 0; i < map.Settings.BoatCount.Length; i++)
        {
            s += map.Settings.BoatCount[i];
            if (i < map.Settings.BoatCount.Length - 1)
                s += ',';
        }
        s += "});\n";
        stream.Write(utf.GetBytes(s));

        int boatTileCount = 0;
        for (int i = 0; i < map.Settings.BoatCount.Length; i++)
            boatTileCount += map.Settings.BoatCount[i] * (i + 1);
        stream.Write(utf.GetBytes("        Int2[] boats = new Int2[" + boatTileCount + "];\n"));

        for (int i = 0; i < boatTileCount; i++)
        {
            stream.Write(utf.GetBytes("        boats[" + i + "] = new Int2("
                                      + map.Boats[i].X + ", " + map.Boats[i].Y + ");\n"));
        }
        
        stream.Write(utf.GetBytes("        return new PreparedMap(s, boats);\n" +
                                  "    }\n}\n"));
        stream.Close();
    }
}