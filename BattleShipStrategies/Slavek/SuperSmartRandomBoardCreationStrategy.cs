using BattleShipEngine;
using BattleShipStrategies.MartinF;
using BattleShipStrategies.Max;
using BattleShipStrategies.Kuba;
using BattleShipStrategies.Slavek.AI;

namespace BattleShipStrategies.Slavek;

public class SuperSmartRandomBoardCreationStrategy : IBoardCreationStrategy
{
    private readonly string _name;
    private readonly IBoardCreationStrategy _smartStrategy;
    private readonly List<IGameStrategy> _enemyStrategies;
    private readonly List<IGameStrategy> _allyStrategies;
    private readonly BoatPaster _paster = new BoatPaster();

    public SuperSmartRandomBoardCreationStrategy(string name="Warrior")
    {
        _name = name;
        _smartStrategy = new SmartRandomBoardCreationStrategy();
        _enemyStrategies = new IGameStrategy[]
        {
            new MartinStrategy(),
            new Strategy_Max(),
            new KubaStrategie()
        }.ToList();
        _allyStrategies = new List<IGameStrategy>();
        if (name != "Solo")
        {
            if (name != "DeathCross")
                _allyStrategies.Add(new DeathCrossStrategy());
            if (name != "AI")
                _allyStrategies.Add(new AIGameStrategy());
        }
    }
    public Int2[] GetBoatPositions(GameSetting setting)
    {
        int mostMoves = 0;
        Int2[] bestResult = new Int2[] {};
        for (int i = 0; i < 10; i++)
        {
            Int2[] newResult = _smartStrategy.GetBoatPositions(setting);
            _paster.SetBoats(newResult);
            int newMoves = 0;
            Game game = new Game(_paster, setting);
            foreach (var gameStrategy in _enemyStrategies)
            {
                newMoves += game.SimulateGame(gameStrategy);
            }
            foreach (var gameStrategy in _allyStrategies)
            {
                newMoves -= game.SimulateGame(gameStrategy);
            }
            if (newMoves > mostMoves)
            {
                bestResult = newResult;
                mostMoves = newMoves;
            }
        }
        return bestResult;
    }
}

internal class BoatPaster : IBoardCreationStrategy
{
    private Int2[] _boats = new Int2[]{};
    public void SetBoats(Int2[] boats)
    {
        _boats = boats;
    }
    
    public Int2[] GetBoatPositions(GameSetting setting)
    {
        return _boats;
    }
}
