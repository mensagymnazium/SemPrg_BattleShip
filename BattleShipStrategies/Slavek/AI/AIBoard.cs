using BattleShipEngine;

namespace BattleShipStrategies.Slavek.AI;

public class AIBoard : IBoardCreationStrategy
{
    private Int2[] _boats;

    public AIBoard(PreparedMap? preparedMap = null)
    {
        if (preparedMap is not null)
            _boats = ((PreparedMap)preparedMap).Boats;
        else
            _boats = PreparedMap.MapSmaDef_MarMax_SDC().Boats;
    }
    
    public Int2[] GetBoatPositions(GameSetting setting)
    {
        return _boats;
    }
}