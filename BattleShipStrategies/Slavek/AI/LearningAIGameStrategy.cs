using BattleShipEngine;

namespace BattleShipStrategies.Slavek.AI;

public class LearningAIGameStrategy : AIGameStrategy
{
    public LearningAIGameStrategy()
    {
        experiencesToChose = new[] {Experiences.General_10_10()};
    }
    
    public override void Start(GameSetting setting)
    {
        if (_board.GetLength(0) > 0)
            _possibleExperiences[0] += _board;
        
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