using BattleShipEngine;

namespace BattleShipStrategies.Slavek.AI;

public class AIGameStrategy : AbstractSlavekStrategy
{
    protected readonly List<Experiences> _possibleExperiences = new ();
    private readonly List<CoefficientMap> _possibleMaps = new ();
    private readonly List<double> _probabilities = new ();
    private int _chosenMapIndex;
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

    protected override Int2 Move()
    {
        if (_problems)
            return _deathCrossStrategy.GetMove();
        
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

    public override void RespondHit()
    {
        if (_problems)
        {
            _deathCrossStrategy.RespondHit();
            return;
        }
        base.RespondHit();
        if (!SetMap())
        {
            _problems = true;
            _deathCrossStrategy.SetEverything(_board);
        }
    }

    public override void RespondSunk()
    {
        if (_problems)
        {
            _deathCrossStrategy.RespondSunk();
            return;
        }
        base.RespondSunk();
        if (!SetMap())
        {
            _problems = true;
            _deathCrossStrategy.SetEverything(_board);
        }
    }

    public override void RespondMiss()
    {
        if (_problems)
        {
            _deathCrossStrategy.RespondMiss();
            return;
        }
        base.RespondMiss();
        if (!SetMap())
        {
            _problems = true;
            _deathCrossStrategy.SetEverything(_board);
        }
    }

    protected override void Draw(Int2 position, SlavekTile result)
    {
        base.Draw(position, result);
        
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
            _possibleMaps[i].Coefficients[position.X, position.Y] = 0;
            _possibleMaps[i] *= (CoefficientMap) change;
        }
    }

    public override void Start(GameSetting setting)
    {
        _problems = false;
        
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
        }
        else
            for (int i = 0; i < _probabilities.Count; i++)
            {
                _probabilities[i] = 1;
                _possibleMaps[i] = _possibleExperiences[i]
                    .InitialCoefficients.CloneCoefficients();
            }
        
        base.Start(setting);

        if (_possibleExperiences.Count == 0)
            _problems = true;
        else
            _chosenMapIndex = 0;

        _deathCrossStrategy.Start(setting);
    }
}
