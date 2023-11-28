using BattleShipEngine;

namespace BattleShipStrategies.Honza;

public class HonzaGameStrategy : IGameStrategy
{
    private GameSetting setting;

    public void Start(GameSetting setting)
    {
        this.setting = setting;
        //everything needs to reset here, this won't be reinitialized
    }

    public Int2 GetMove()
    {
        return new Int2(
            Random.Shared.Next(setting.Width),
            Random.Shared.Next(setting.Height)
        );
    }

    public void RespondMiss()
    {
    }

    public void RespondHit()
    {
    }

    public void RespondSunk()
    {
    }
}