using BattleShipEngine;
using BattleShipsAnalytics.Tournaments;
using BattleShipExternalStrategies;
using BattleShipStrategies.Default;
using BattleShipStrategies.MartinF;
using BattleShipStrategies.MartinF.Unethical;
using BattleShipStrategies.Honza;
using BattleShipStrategies.Slavek;
using BattleShipStrategies.Robert;
using BattleShipStrategies.Max;
using BattleShipStrategies.Tobias;
using BattleShipStrategies.Slavek.AI;


var participants = new List<Participant>()
{
	new("MartinF", new MartinBoardCreationStrategy(), new MartinStrategy()),
    //new("Legit100%NoCap", new SmartRandomBoardCreationStrategy(), new MartinParasiticStrategy()),
    new("SmartRandom", new SmartRandomBoardCreationStrategy(), new SmartRandomStrategy()),
    new("Slavek", new SuperSmartRandomBoardCreationStrategy("DeathCross"), new DeathCrossStrategy()),
    new("Slavek-AI", new SuperSmartRandomBoardCreationStrategy("AI"), new AIGameStrategy()),
    new("Slavek-LAI", new SuperSmartRandomBoardCreationStrategy(), new LearningAIGameStrategy()),
    //new("External", new ExternalBoardCreationStrategy(65431), new ExternalGameStrategy(65432)),
    //new("Honza", new HonzaBoardCreationStrategy(), new HonzaGameStrategy()),
    new("Robert+S_ChatGpt1", new ChatGptBoardCreationStrategy(), new ChatGpt1GameStrategy()),
	new("Robert+S_ChatGpt2", new ChatGptBoardCreationStrategy(), new ChatGpt2GameStrategy()),
	new("Robert+S_GHC+CGPT", new ChatGptBoardCreationStrategy(), new GitHubCopilotGameStrategy()),
    //new("Honza", new HonzaBoardCreationStrategy(), new HonzaGameStrategy()),
    new("Tobias", new TobiasBoardCreationStrategy(), new TobiasGameStrategy()),

    //new("Interactive", new InteractiveBoardCreationStrategy(), new InteractiveGameStrategy())
};

var settings = GameSetting.Default;

//var tournament = new SingleShotTournament(participants);
var tournament = new MultiGameTournament(participants, 100);
//var tournament = new MultiThreadedTournament(participants, 1000); //Might be faster, but not sure (lol)

tournament.PlayAndPrint(settings);

// Check for external strategies and say them to close sockets.
// Say LearningAIGameStrategy to save new knowledge.
foreach (Participant participant in participants)
{
	if (participant.GameStrategy is ExternalGameStrategy)
		participant.GameStrategy.Start(new GameSetting(
			0, 0, new int[] { }));
/*
	else if (participant.GameStrategy is LearningAIGameStrategy)
		participant.GameStrategy.Start(new GameSetting(
			0, 0, new int[] { }));
*/

	if (participant.BoardCreationStrategy is ExternalBoardCreationStrategy)
		participant.BoardCreationStrategy.GetBoatPositions(
			new GameSetting(0, 0, new int[] { }));

	// Start empty = Exit
}
