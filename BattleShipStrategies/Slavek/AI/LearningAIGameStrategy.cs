using BattleShipEngine;

namespace BattleShipStrategies.Slavek.AI;

public class LearningAIGameStrategy : AIGameStrategy
{
    private uint _roundCounter;
    private readonly uint _roundLength;
    
    public LearningAIGameStrategy(uint roundLength=100)
    {
        experiencesToChose = new[] {Experiences.General_10_10(),
            Experiences.Blank("Placeholder", new GameSetting(0, 0, new []{0}))};
        _roundLength = roundLength;
        _roundCounter = roundLength;
    }

    public LearningAIGameStrategy() : this(100)
    {}
    
    public override void Start(GameSetting setting)
    {
        if (_roundCounter == _roundLength)
        {
            experiencesToChose[1] = Experiences.Blank("ThisRound", setting);
            base.Start(new GameSetting(0, 0, new []{0}));
            _roundCounter = 0;
        }
        _roundCounter++;

        if (_board.GetLength(0) > 0)
        {
            _possibleExperiences[0] += _board;
            _possibleExperiences[1] += _board;
        }
        
        if (setting.Width == 0 || setting.Height == 0)
        {
            Trainer.WriteToData(
                "General_" +_setting.Width.ToString() + '_' + _setting.Height.ToString(),
                _possibleExperiences[0]);
            return;
        }
        
        base.Start(setting);
    }
}