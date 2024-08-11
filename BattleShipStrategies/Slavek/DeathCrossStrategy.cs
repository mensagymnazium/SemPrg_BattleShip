using BattleShipEngine;
using BattleShipStrategies.Default;

namespace BattleShipStrategies.Slavek;

public class DeathCrossStrategy : IGameStrategy
{
    private readonly List<List<Int2>> _stablePlaces = new();
    private readonly List<IBoardCreationStrategy> _stableStrategies;
    private SlavekTile[,] _board = new SlavekTile[0,0];
    private GameSetting _setting;
    private Int2 _lastMove;
    private readonly List<Int2> _deathCross = new();
    private bool _hunter;
    private (Int2 from, Int2 to) _bleeding;

    public DeathCrossStrategy()
    {
        _stableStrategies = new();
    }

    public DeathCrossStrategy(List<IBoardCreationStrategy> strategies)
    {
        _stableStrategies = strategies;
    }
    
    public Int2 GetMove()
    {
        Int2 move = Move();
        _lastMove = move;
        return move;
    }

    private Int2 Move()
    {
        if (_hunter)
        {
            for (int i = _stablePlaces.Count - 1; i >= 0; i--)
                if (!((_bleeding.from.Y > 0 && _stablePlaces[i].Contains(
                        _bleeding.from with { Y = _bleeding.from.Y - 1 }))
                      || (_bleeding.to.Y < _setting.Height - 1 && _stablePlaces[i].Contains(
                          _bleeding.to with { Y = _bleeding.to.Y + 1 }))
                      || (_bleeding.from.X > 0 && _stablePlaces[i].Contains(
                          _bleeding.from with { X = _bleeding.from.X - 1 }))
                      || (_bleeding.to.X < _setting.Width - 1 && _stablePlaces[i].Contains(
                          _bleeding.to with { X = _bleeding.to.X + 1 }))))
                    _stablePlaces.RemoveAt(i);
            if (_stablePlaces.Any())
            {
                if (_bleeding.from.Y > 0 && _stablePlaces[0].Contains(
                        _bleeding.from with { Y = _bleeding.from.Y - 1 }))
                    return _bleeding.from with { Y = _bleeding.from.Y - 1 };
                if (_bleeding.to.Y < _setting.Height - 1 && _stablePlaces[0].Contains(
                             _bleeding.to with { Y = _bleeding.to.Y + 1 }))
                    return _bleeding.to with { Y = _bleeding.to.Y + 1 };
                if (_bleeding.from.X > 0 && _stablePlaces[0].Contains(
                        _bleeding.from with { X = _bleeding.from.X - 1 }))
                    return _bleeding.from with { X = _bleeding.from.X - 1 };
                if (_bleeding.to.X < _setting.Width - 1 && _stablePlaces[0].Contains(
                        _bleeding.to with { X = _bleeding.to.X + 1 }))
                    return _bleeding.to with { X = _bleeding.to.X + 1 };
            }
            if (_bleeding.from == _bleeding.to || _bleeding.from.Y == _bleeding.to.Y)
            {
                if (_bleeding.from.X > 0 &&
                    _board[_bleeding.from.X - 1, _bleeding.from.Y] == SlavekTile.Unknown)
                    return _bleeding.from with { X = _bleeding.from.X - 1 };
                if (_bleeding.to.X < _setting.Width - 1 &&
                    _board[_bleeding.to.X + 1, _bleeding.to.Y] == SlavekTile.Unknown)
                    return _bleeding.to with { X = _bleeding.to.X + 1 };
            }
            if (_bleeding.from == _bleeding.to || _bleeding.from.X == _bleeding.to.X)
            {
                if (_bleeding.from.Y > 0 &&
                    _board[_bleeding.from.X, _bleeding.from.Y - 1] == SlavekTile.Unknown)
                    return _bleeding.from with { Y = _bleeding.from.Y - 1 };
                if (_bleeding.to.Y < _setting.Height - 1 &&
                    _board[_bleeding.to.X, _bleeding.to.Y + 1] == SlavekTile.Unknown)
                    return _bleeding.to with { Y = _bleeding.to.Y + 1 };
            } 
        }

        Int2 best = new Int2(0, 0);
        int coef = 0;
        for (int i = 0; i < _setting.Width; i++)
        for (int j = 0; j < _setting.Height; j++)
        {
            if (_board[i, j] != SlavekTile.Unknown)
                continue;
            Int2 place = new Int2(i, j);
            int newCoef = 0;
            if (_deathCross.Contains(place))
                newCoef++;
            foreach (List<Int2> places in _stablePlaces)
                if (places.Contains(place))
                    newCoef+=2;
            if (newCoef > coef)
            {
                coef = newCoef;
                best = place;
            }
        }
        if (coef > 0)
            return best;
        //Death cross finished.
        for (int i = 0; i < _setting.Width; i++)
        for (int j = 0; j < _setting.Height; j++)
        {
            if (_board[i, j] != SlavekTile.Unknown)
                continue;
            int newCoef = 1;
            for (int k = 0; k < 4; k++)
                newCoef += EmptyCount(0, new Int2(i, j), (Direction)k);
            if (newCoef > coef)
            {
                coef = newCoef;
                best = new Int2(i, j);
            }
        }
        return best;
    }
    
    private int EmptyCount(int depth, Int2 position, Direction direction)
    {
        if (depth == _setting.BoatCount.Length)
            return depth;
        if (_board[position.X, position.Y] == SlavekTile.Unknown)
        {
            depth ++;
            if (direction == Direction.Left && position.X > 0)
                depth = EmptyCount(depth, position with { X = position.X - 1 }, direction);
            else if (direction == Direction.Up && position.Y > 0)
                depth = EmptyCount(depth, position with { Y = position.Y - 1 }, direction);
            else if (direction == Direction.Right && position.X < _setting.Width - 1)
                depth = EmptyCount(depth, position with { X = position.X + 1 }, direction);
            else if (direction == Direction.Down && position.Y < _setting.Height - 1)
                depth = EmptyCount(depth, position with { Y = position.Y + 1 }, direction);
        }
        return depth;
    }

    public void RespondHit()
    {
        //Console.WriteLine("Hit!");
        if (_hunter)
        {
            if (_lastMove.X < _bleeding.from.X || _lastMove.Y < _bleeding.from.Y)
                _bleeding.from = _lastMove;
            else if (_lastMove.X > _bleeding.to.X || _lastMove.Y > _bleeding.to.Y)
                _bleeding.to = _lastMove;
        }
        else
        {
            _hunter = true;
            _bleeding = (_lastMove, _lastMove);
        }
        _board[_lastMove.X, _lastMove.Y] = SlavekTile.DamagedBoat;
    }

    public void RespondSunk()
    {
        //Console.WriteLine("SUNK!!!");
        _hunter = false;
        for (int i = 0; i < 4; i++)
            BoatIsDead(_lastMove, (Direction) i);
    }
    
    private void BoatIsDead(Int2 position, Direction direction)
    {
        if (direction == Direction.Left || direction == Direction.Right)
        {
            if (position.Y < _setting.Height - 1
                && _board[position.X, position.Y + 1] == SlavekTile.Unknown)
                _board[position.X, position.Y + 1] = SlavekTile.Water;
            if (position.Y > 0
                && _board[position.X, position.Y - 1] == SlavekTile.Unknown)
                _board[position.X, position.Y - 1] = SlavekTile.Water;
        }
        if (direction == Direction.Up || direction == Direction.Down)
        {
            if (position.X < _setting.Width - 1
                && _board[position.X + 1, position.Y] == SlavekTile.Unknown)
                _board[position.X + 1, position.Y] = SlavekTile.Water;
            if (position.X > 0
                && _board[position.X - 1, position.Y] == SlavekTile.Unknown)
                _board[position.X - 1, position.Y] = SlavekTile.Water;
        }
        
        if (_board[position.X, position.Y] == SlavekTile.Unknown)
            _board[position.X, position.Y] = SlavekTile.Water;
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

    public void RespondMiss()
    {
        //Console.WriteLine("Miss.");
        _board[_lastMove.X, _lastMove.Y] = SlavekTile.Water;
        if (_stablePlaces.Any())
        {
            List<List<Int2>> toRemove = new();
            foreach (List<Int2> places in _stablePlaces)
                if (places.Contains(_lastMove))
                    toRemove.Add(places);
            foreach (List<Int2> places in toRemove)
                _stablePlaces.Remove(places);
        }
    }

    public void Start(GameSetting setting)
    {
        _board = new SlavekTile[setting.Width, setting.Height];
        if (_setting != setting)
        {
            _setting = setting;
            _deathCross.Clear();
            int mySum = Math.Min(setting.Width, setting.Height);
            for (int i = 0; i < mySum; i++)
            {
                _deathCross.Add(new Int2(i, i));
                _deathCross.Add(new Int2(i, mySum - 1 - i));
            }
        }
        _stablePlaces.Clear();
        for (int i = 0; i < _stableStrategies.Count; i++)
            _stablePlaces.Add(_stableStrategies[i].GetBoatPositions(setting).ToList());
        _hunter = false;
    }

    internal void SetEverything(SlavekTile[,] board)
    {
        _board = board;
        _stablePlaces.Clear();
    }
}