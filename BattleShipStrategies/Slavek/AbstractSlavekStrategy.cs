using BattleShipEngine;

namespace BattleShipStrategies.Slavek;

public abstract class AbstractSlavekStrategy : IGameStrategy
{
    protected SlavekTile[,] _board = new SlavekTile[0,0];
    protected GameSetting _setting;
    protected Int2 _lastMove;

    public virtual void Start(GameSetting setting)
    {
        _setting = setting;
        _board = new SlavekTile[setting.Width, setting.Height];
    }

    public virtual Int2 GetMove()
    {
        Int2 move = Move();
        _lastMove = move;
        return move;
    }

    protected virtual Int2 Move()
    {
        return new Int2(Random.Shared.Next(_setting.Width), Random.Shared.Next(_setting.Height));
    }

    protected virtual void Draw(Int2 position, SlavekTile result)
    {
        _board[position.X, position.Y] = result;
    }

    public virtual void RespondMiss()
    {
        Draw(_lastMove, SlavekTile.Water);
    }

    public virtual void RespondHit()
    {
        Draw(_lastMove, SlavekTile.Boat);
    }

    public virtual void RespondSunk()
    {
        for (int i = 0; i < 4; i++)
            BoatIsDead(_lastMove, (Direction) i);
    }

    private void BoatIsDead(Int2 position, Direction direction)
    {
        if (direction == Direction.Left || direction == Direction.Right)
        {
            if (position.Y < _setting.Height - 1
                && _board[position.X, position.Y + 1] == SlavekTile.Unknown)
                Draw(position with { Y = position.Y + 1 }, SlavekTile.Water);
            if (position.Y > 0
                && _board[position.X, position.Y - 1] == SlavekTile.Unknown)
                Draw(position with { Y = position.Y - 1 }, SlavekTile.Water);
        }
        if (direction == Direction.Up || direction == Direction.Down)
        {
            if (position.X < _setting.Width - 1
                && _board[position.X + 1, position.Y] == SlavekTile.Unknown)
                Draw(position with { X = position.X + 1 }, SlavekTile.Water);
            if (position.X > 0
                && _board[position.X - 1, position.Y] == SlavekTile.Unknown)
                Draw(position with { X = position.X - 1 }, SlavekTile.Water);
        }
        
        if (_board[position.X, position.Y] == SlavekTile.Unknown)
            Draw(position, SlavekTile.Water);
        if (_board[position.X, position.Y] == SlavekTile.Water)
            return;
        
        if (position.X > 0 && direction == Direction.Left)
            BoatIsDead(position with {X = position.X - 1}, direction);
        if (position.X < _setting.Width - 1 && direction == Direction.Right)
            BoatIsDead(position with {X = position.X + 1}, direction);
        if (position.Y > 0 && direction == Direction.Up)
            BoatIsDead(position with {Y = position.Y - 1}, direction);
        if (position.Y < _setting.Height - 1 && direction == Direction.Down)
            BoatIsDead(position with {Y = position.Y + 1}, direction);
    }
}