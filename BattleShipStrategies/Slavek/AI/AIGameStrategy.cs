using BattleShipEngine;

namespace BattleShipStrategies.Slavek.AI;

public class AIGameStrategy : IGameStrategy
{
    protected SlavekTile[,] _board = new SlavekTile[0,0];
    protected readonly List<Experiences> _possibleExperiences = new ();
    private readonly List<CoefficientMap> _possibleMaps = new ();
    protected GameSetting _setting;
    private readonly List<double> _probabilities = new ();
    private int _chosenMapIndex;
    private Int2 _lastMove;
    private readonly DeathCrossStrategy _deathCrossStrategy = new DeathCrossStrategy();
    private bool _problems;
    protected Experiences[] experiencesToChose = new[]
    {
        Experiences.DefaultSmartRandom(), Experiences.SmallSmartRandom(),
        Experiences.LargeSmartRandom(), Experiences.HasbroSmartRandom(),
        Experiences.DefaultMartin(), Experiences.DefaultDefault(),
        Experiences.DefaultMax(), Experiences.DefaultChatGPT(),
        Experiences.HasbroMartin(), Experiences.HasbroDefault(),
        Experiences.HasbroChatGPT(), Experiences.LargeMartin(),
        Experiences.LargeDefault(), Experiences.LargeChatGPT(),
        Experiences.SmallDefault(), Experiences.SmallChatGPT(),
        Experiences.DefaultKuba(), Experiences.DefaultMarKubMax(),
    };
    
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
            if (_possibleMaps[_chosenMapIndex].Coefficients[i, j] > bestCoef)
            {
                bestMove = new Int2(i, j);
                bestCoef = _possibleMaps[_chosenMapIndex].Coefficients[i, j];
            }
        }

        if (bestCoef > 0)
            return bestMove;
        
        else
        {
            _probabilities[_chosenMapIndex] = 0;
            if (!SetMap())
            {
                _problems = true;
                _deathCrossStrategy.SetEverything(_board);
                return _deathCrossStrategy.GetMove();
            }
            return Move();
        }
    }

    private bool SetMap()
    {
        bool anyUsable = false;
        for (int i = 0; i < _probabilities.Count; i++)
        {
            if (_probabilities[i] > 0)
                anyUsable = true;
            if (_probabilities[_chosenMapIndex] < _probabilities[i])
                _chosenMapIndex = i;
        }
        return anyUsable;
    }

    public void RespondHit()
    {
        if (_problems)
        {
            _deathCrossStrategy.RespondHit();
            return;
        }
        if (!UpdateMaps(_lastMove, SlavekTile.Boat, true))
        {
            _problems = true;
            _deathCrossStrategy.SetEverything(_board);
        }
    }

    public void RespondSunk()
    {
        for (int i = 0; i < 4; i++)
            if (!BoatIsDead(_lastMove, (Direction) i))
            {
                _problems = true;
                _deathCrossStrategy.SetEverything(_board);
            }
    }
    
    private bool BoatIsDead(Int2 position, Direction direction)
    {
        if (direction == Direction.Left || direction == Direction.Right)
        {
            if (position.Y < _setting.Height - 1
                && _board[position.X, position.Y + 1] == SlavekTile.Unknown)
                UpdateMaps(position with { Y = position.Y + 1 }, SlavekTile.Water);
            if (position.Y > 0
                && _board[position.X, position.Y - 1] == SlavekTile.Unknown)
                UpdateMaps(position with { Y = position.Y - 1 }, SlavekTile.Water);
        }
        if (direction == Direction.Up || direction == Direction.Down)
        {
            if (position.X < _setting.Width - 1
                && _board[position.X + 1, position.Y] == SlavekTile.Unknown)
                UpdateMaps(position with { X = position.X + 1 }, SlavekTile.Water);
            if (position.X > 0
                && _board[position.X - 1, position.Y] == SlavekTile.Unknown)
                UpdateMaps(position with { X = position.X - 1 }, SlavekTile.Water);
        }
        
        if (_board[position.X, position.Y] == SlavekTile.Unknown)
            UpdateMaps(position, SlavekTile.Water);
        if (_board[position.X, position.Y] == SlavekTile.Water)
            return SetMap();
        
        if (position.X > 0 && direction == Direction.Left)
            BoatIsDead(position with {X = position.X - 1}, direction);
        if (position.X < _setting.Width - 1 && direction == Direction.Right)
            BoatIsDead(position with {X = position.X + 1}, direction);
        if (position.Y > 0 && direction == Direction.Up)
            BoatIsDead(position with {Y = position.Y - 1}, direction);
        if (position.Y < _setting.Height - 1 && direction == Direction.Down)
            BoatIsDead(position with {Y = position.Y + 1}, direction);
        
        return SetMap();
    }

    public void RespondMiss()
    {
        if (_problems)
        {
            _deathCrossStrategy.RespondMiss();
            return;
        }
        if (!UpdateMaps(_lastMove, SlavekTile.Water, true))
        {
            _problems = true;
            _deathCrossStrategy.SetEverything(_board);
        }
    }

    private void SetZero(Int2 position, SlavekTile result)
    {
        _board[position.X, position.Y] = result;
        foreach (var map in _possibleMaps)
            map.Coefficients[position.X, position.Y] = 0;
    }

    private bool UpdateMaps(Int2 position, SlavekTile result, bool alsoSetMap=false)
    {
        bool anyProbable = false;
        for (int i = 0; i < _probabilities.Count; i++)
        {
            if (_probabilities[i] == 0)
                continue;
            if (result == SlavekTile.Boat)
                _probabilities[i] *= _possibleMaps[i].Coefficients[position.X, position.Y];
            else if (_possibleMaps[i].Coefficients[position.X, position.Y] < 1)
                _probabilities[i] *= 1 - _possibleMaps[i].Coefficients[position.X, position.Y];
            CoefficientMap? change = _possibleExperiences[i].Changes[(position, result)];
            if (change is null)
            {
                _probabilities[i] = 0;
                continue;
            }
            _possibleMaps[i] *= (CoefficientMap) change;
            anyProbable = true;
        }

        SetZero(position, result);
        if (!anyProbable)
            return false;
        if (alsoSetMap)
            SetMap();
        return true;
    }

    public virtual void Start(GameSetting setting)
    {
        _problems = false;
        _board = new SlavekTile[setting.Width, setting.Height];
        
        if (setting != _setting)
        {
            _possibleExperiences.Clear();
            _possibleMaps.Clear();
            _probabilities.Clear();
            foreach (Experiences experience in experiencesToChose)
                if (GameSetting.AreSame(experience.Settings, setting))
                {
                    _possibleExperiences.Add(experience);
                    _possibleMaps.Add(experience.InitialCoefficients.CloneCoefficients());
                    _probabilities.Add(1);
                }
            
            _setting = setting;
        }
        else
            for (int i = 0; i < _probabilities.Count; i++)
            {
                _probabilities[i] = 1;
                _possibleMaps[i] = _possibleExperiences[i]
                    .InitialCoefficients.CloneCoefficients();
            }

        if (_possibleExperiences.Count == 0)
            _problems = true;
        else
            _chosenMapIndex = 0;

        _deathCrossStrategy.Start(setting);
    }
}
