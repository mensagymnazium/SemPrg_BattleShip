using BattleShipEngine;

namespace BattleShipsAnalytics.Tournaments;

internal class MultiGameTournament : AbstractTournament
{
    public MultiGameTournament(List<Participant> participants, int gamesPerBoard)
        : base(participants)
    {
        _gamesPerBoard = gamesPerBoard;
    }

    public override void PlayAndPrint(GameSetting settings)
    {
        //Setup scores
        var competitorsScores = new Dictionary<Participant, int>(); // Key: Participant, Value: How many moves it took to sink all boats
        foreach (var participant in _participants)
        {
            competitorsScores.Add(participant, 0);
        }

        foreach (var participant in _participants)
        {
            Console.WriteLine($"The {_gamesPerBoard} boards of {participant.Name}:");
            var currentTotalMoves = new Dictionary<Participant, int>();
            foreach (var competitor in _participants)
            {
                if (competitor == participant)
                    continue; //The player shouldn't play against himself
                currentTotalMoves.Add(competitor, 0);
            }

            for (int i = 0; i < _gamesPerBoard; i++)
            {
                //Assume boards are valid (lol) (it's faster)
                var game = new Game(participant.BoardCreationStrategy, settings);
            
                //Calculate how others did against this board
                foreach (var competitor in _participants)
                {
                    if (competitor == participant)
                        continue; //The player shouldn't play against himself

                    //Simulate games on this board
                    var ammOfMoves = game.SimulateGame(competitor.GameStrategy);
                    competitorsScores[competitor] += ammOfMoves;
                    currentTotalMoves[competitor] += ammOfMoves;
                }
            }

            foreach (var competitor in _participants)
            {
                if (competitor == participant)
                    continue;
                Console.WriteLine($"\t-{competitor.Name}: {
                    currentTotalMoves[competitor]} moves - avg: {
                        currentTotalMoves[competitor] / (double)_gamesPerBoard}");
            }
            Console.WriteLine();
        }

        DrawResultTable(competitorsScores);
    }
}