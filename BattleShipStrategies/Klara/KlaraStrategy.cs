using BattleShipEngine;

namespace BattleShipStrategies.Klara;

public class KlaraStrategy : IGameStrategy
{
    private GameSetting setting;

    public Int2 GetMove()
    {
        int x = 0, y = 0;
        if (x <= setting.Width && y<= setting.Height)
        {
            return new Int2(
                x, y
            );
            x++; y++;
        }
        else if (x==setting.Width)
        {
            return new Int2( 0, y+1 );
            x = 0; y++;
        }
        else { return new Int2(setting.Width, setting.Height); }
        
    }

    public void RespondHit()
    {
    }

    public void RespondSunk()
    {
    }

    public void RespondMiss()
    {
    }

    public void Start(GameSetting setting)
    {
        this.setting = setting;
    }
}
