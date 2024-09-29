using BattleShipEngine;
using BattleShipStrategies.Default;

namespace BattleShipStrategies.Slavek;

public class DeathCrossStrategy : AbstractSlavekStrategy
{
    private readonly List<List<Int2>> _stablePlaces = new();
    private readonly List<IBoardCreationStrategy> _stableStrategies;
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

    protected override Int2 Move()
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

    public override void RespondHit()
    {
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
        base.RespondHit();
        if (_stablePlaces.Any())
        {
            List<List<Int2>> toRemove = new();
            foreach (List<Int2> places in _stablePlaces)
                if (!places.Contains(_lastMove))
                    toRemove.Add(places);
            foreach (List<Int2> places in toRemove)
                _stablePlaces.Remove(places);
        }
    }

    public override void RespondSunk()
    {
        _hunter = false;
        base.RespondSunk();
    }

    public override void RespondMiss()
    {
        base.RespondMiss();
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

    public override void Start(GameSetting setting)
    {
        if (_setting != setting)
        {
            _deathCross.Clear();
            int mySum = Math.Min(setting.Width, setting.Height);
            for (int i = 0; i < mySum; i++)
            {
                _deathCross.Add(new Int2(i, i));
                _deathCross.Add(new Int2(i, mySum - 1 - i));
            }
        }
        base.Start(setting);
        _stablePlaces.Clear();
        foreach (var strategy in _stableStrategies)
            _stablePlaces.Add(strategy.GetBoatPositions(setting).ToList());
        _hunter = false;
    }

    internal void SetEverything(SlavekTile[,] board)
    {
        _board = board;
        _stablePlaces.Clear();
    }
}