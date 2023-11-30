using BattleShipEngine;

namespace BattleShipStrategies.Slavek.AI;

public class AIGameStrategy : IGameStrategy
{
    private CoefficientMap _map;
    private SlavekTile[,] _board = new SlavekTile[0,0];
    private List<Experiences> _possibleExperiences = new ();
    private Experiences _experiences;
    private GameSetting _setting;
    private Int2 _lastMove;
    private readonly DeathCrossStrategy _deathCrossStrategy = new DeathCrossStrategy();
    private bool _problems = false;
    
    public Int2 GetMove()
    {
        if (_problems)
            return _deathCrossStrategy.GetMove();
        Int2 move = Move();
        _lastMove = move;
        return move;
    }

    private Int2 Move()
    {
        double bestCoef = 0;
        Int2 bestMove = new Int2(0, 0);
        for (int i = 0; i < _setting.Width; i++)
        for (int j = 0; j < _setting.Height; j++)
        {
            if (_map.Coefficients[i, j] > bestCoef)
            {
                bestMove = new Int2(i, j);
                bestCoef = _map.Coefficients[i, j];
            }
        }
        return bestMove;
    }

    public void RespondHit()
    {
        if (_problems)
        {
            _deathCrossStrategy.RespondHit();
            return;
        }
        _board[_lastMove.X, _lastMove.Y] = SlavekTile.DamagedBoat;
        _map.Coefficients[_lastMove.X, _lastMove.Y] = 0;
        CoefficientMap? change = _experiences.Changes[(_lastMove, SlavekTile.Boat)];
        if (change is null)
        {
            _problems = true;
            _deathCrossStrategy.SetEverything(_board);
        }
        else
            _map += (CoefficientMap) change;
    }

    public void RespondSunk()
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
            {
                _board[position.X, position.Y + 1] = SlavekTile.Water;
                _map.Coefficients[position.X, position.Y + 1] = 0;
            }
            if (position.Y > 0
                && _board[position.X, position.Y - 1] == SlavekTile.Unknown)
            {
                _board[position.X, position.Y - 1] = SlavekTile.Water;
                _map.Coefficients[position.X, position.Y - 1] = 0;
            }
        }
        if (direction == Direction.Up || direction == Direction.Down)
        {
            if (position.X < _setting.Width - 1
                && _board[position.X + 1, position.Y] == SlavekTile.Unknown)
            {
                _board[position.X + 1, position.Y] = SlavekTile.Water;
                _map.Coefficients[position.X + 1, position.Y] = 0;
            }
            if (position.X > 0
                && _board[position.X - 1, position.Y] == SlavekTile.Unknown)
            {
                _board[position.X - 1, position.Y] = SlavekTile.Water;
                _map.Coefficients[position.X - 1, position.Y] = 0;
            }
        }
        
        if (_board[position.X, position.Y] == SlavekTile.Unknown)
        {
            _board[position.X, position.Y] = SlavekTile.Water;
            _map.Coefficients[position.X, position.Y] = 0;
        }
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
        if (_problems)
        {
            _deathCrossStrategy.RespondMiss();
            return;
        }
        _board[_lastMove.X, _lastMove.Y] = SlavekTile.Water;
        _map.Coefficients[_lastMove.X, _lastMove.Y] = 0;
        CoefficientMap? change = _experiences.Changes[(_lastMove, SlavekTile.Water)];
        if (change is null)
        {
            _problems = true;
            _deathCrossStrategy.SetEverything(_board);
        }
        else
            _map += (CoefficientMap) change;
    }

    public void Start(GameSetting setting)
    {
        _problems = false;
        _board = new SlavekTile[setting.Width, setting.Height];
        
        if (setting != _setting)
        {
            _possibleExperiences = new List<Experiences>();
            foreach (Experiences experience in new[]
                     {
                         Experiences.DefaultSmartRandom(), Experiences.SmallSmartRandom(),
                         Experiences.LargeSmartRandom(), Experiences.HasbroSmartRandom()
                     })
                if (GameSetting.AreSame(experience.Settings, setting))
                    _possibleExperiences.Add(experience);
            
            _setting = setting;
        }

        if (_possibleExperiences.Count == 0)
            _problems = true;
        else
        {

            _experiences = _possibleExperiences[0];
            _map = _experiences.InitialCoefficients.CloneCoefficients();
        }

        _deathCrossStrategy.Start(setting);
    }
}