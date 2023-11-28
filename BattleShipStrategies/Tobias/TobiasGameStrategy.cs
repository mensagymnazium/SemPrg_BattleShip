using BattleShipEngine;

namespace BattleShipStrategies.Default;

public class TobiasGameStrategy : IGameStrategy
{
    private GameSetting setting;

    private List<Int2> clearedFields;
    private List<Int2> shotHistory;
    private Queue<Int2> futureTargets;

    public Int2 GetMove()
    {
        Int2 target;

        if (futureTargets.Count() != 0)
        {
            target = futureTargets.Dequeue();
        }
        else
        {
            do
            {
                target = new Int2(
                    Random.Shared.Next(setting.Width),
                    Random.Shared.Next(setting.Height)
                );
            }
            while (clearedFields.Contains(target));
        }

        clearedFields.Add(target);
        shotHistory.Add(target);
        return target;
    }

    public void RespondHit()
    {
        var x = shotHistory.Last().X;
        var y = shotHistory.Last().Y;

        if (x > 0)
        {
            futureTargets.Enqueue(new Int2(x - 1, y));
        }
        if (x < setting.Width - 1)
        {
            futureTargets.Enqueue(new Int2(x + 1, y));
        }
        if (y > 0)
        {
            futureTargets.Enqueue(new Int2(x, y - 1));
        }
        if (y < setting.Height - 1)
        {
            futureTargets.Enqueue(new Int2(x, y + 1));
        }
    }

    public void RespondSunk()
    {
        futureTargets.Clear();
    }

    public void RespondMiss()
    {
    }

    public void Start(GameSetting setting)
    {
        this.setting = setting;
        clearedFields = new List<Int2>();
        shotHistory = new List<Int2>();
        futureTargets = new Queue<Int2>();
    }
}