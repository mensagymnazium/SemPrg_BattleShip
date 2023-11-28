using BattleShipEngine;

namespace BattleShipStrategies.Slavek.AI;

public readonly partial record struct Experiences(string StrategyName, GameSetting Settings,
    CoefficientMap InitialCoefficients, Dictionary<(Int2, SlavekTile), CoefficientMap?> Changes)
{}

public readonly partial record struct Experiences
{
    public void AddChange(Int2 position, SlavekTile shot, CoefficientMap? changeMap)
    {
        Changes.Add((position, shot), changeMap);
    }
}
