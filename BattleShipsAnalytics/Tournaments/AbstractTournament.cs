using BattleShipEngine;

namespace BattleShipsAnalytics.Tournaments;

public abstract class AbstractTournament : ITournament
{
    protected readonly List<Participant> _participants;
    protected int _gamesPerBoard = 1;
    protected AbstractTournament(List<Participant> participants)
    {
        _participants = participants;
    }
    
    public virtual void PlayAndPrint(GameSetting settings)
    {}
    
    protected virtual void DrawResultTable(Dictionary<Participant, int> competitorsScores)
    {
        //Final results
        //Draw it as a table with -+| and stuff
        const int nameWidth = 20;
        const int avgWidth = 10;
        const int totalWidth = 20;

        Console.WriteLine("\nTotal amount of moves needed to solve all the opponents' boards:");
        Console.WriteLine($"{"Name",-nameWidth}|{"Total",-totalWidth}|{"Avg",-avgWidth}");
        Console.WriteLine($"{"".PadRight(nameWidth, '-')}+{"".PadRight(totalWidth, '-')}+{"".PadRight(avgWidth, '-')}");
        foreach (var participant in
                 competitorsScores.OrderBy(x => x.Value))
        {
            var avg = participant.Value / (double)_gamesPerBoard / (_participants.Count-1); // -1 because we don't count the participant himself
            Console.WriteLine(
                $"{participant.Key.Name,-nameWidth}|{participant.Value,-totalWidth}|{avg}");
        }
    }
}