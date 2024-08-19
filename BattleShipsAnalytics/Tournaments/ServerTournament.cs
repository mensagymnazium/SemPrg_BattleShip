using System.Net;
using System.Net.Sockets;
using System.Text;
using BattleShipEngine;

namespace BattleShipsAnalytics.Tournaments;

public class ServerTournament : AbstractTournament
{
    private List<TournamentConnection> _connections = new ();
    public IPAddress IPAddress { get; set; }

    public ServerTournament(List<Participant> participants, int gamesPerBoard, IPAddress? ipAddress = null)
        : base(participants)
    {
        _gamesPerBoard = gamesPerBoard;
        if (ipAddress is null)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            ipAddress = ipHost.AddressList[0];
        }
        IPAddress = ipAddress;
    }

    public override void PlayAndPrint(GameSetting settings)
    {
        Console.WriteLine("Waiting for all games to finish...");

        Task.Run(() => PlayAndPrintAsync(settings)).Wait();
    }
    
    public async Task PlayAndPrintAsync(GameSetting settings)
    {
        foreach (var connection in _connections)
            connection.SendSetting(settings);
        
        var competitorsScores = new Dictionary<Participant, int>();
        foreach (var participant in _participants)
            competitorsScores.Add(participant, 0);

        foreach (var participant in _participants) //Local participants only
        {
            Console.WriteLine($"Boards of {participant.Name}");
            var currentTotalMoves = new Dictionary<Participant, int>();
            foreach (var competitor in _participants)
            {
                if (competitor == participant)
                    continue; //The player shouldn't play against himself
                currentTotalMoves.Add(competitor, 0);
            }

            for (int i = 0; i < _gamesPerBoard; i++)
            {
                Int2[] boatPositions = participant.BoardCreationStrategy.GetBoatPositions(settings);
                var tasks = new Task[_connections.Count + 1];

                for (int j = 0; j < _connections.Count; j++)
                {
                    var connection = _connections[j];
                    var task = Task.Run(() => connection.PlayRound(boatPositions));
                    tasks[j] = task;
                }
                
                //Simulate the local games
                var local = Task.Run(() =>
                {
                    Game game = new(boatPositions, settings);
                    foreach (var competitor in _participants)
                    {
                        if (competitor == participant)
                            continue;

                        int ammOfMoves = game.SimulateGame(competitor.GameStrategy);
                        competitorsScores[competitor] += ammOfMoves;
                        currentTotalMoves[competitor] += ammOfMoves;
                    }
                });
                tasks[_connections.Count] = local;
                
                await Task.WhenAll(tasks);
            }
            
            //Write results of local competitors against this local participant
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

        Console.WriteLine("Local results for local boards:");
        DrawResultTable(competitorsScores);
        Console.WriteLine();
        
        //Play games on boards from external tournaments
        foreach (var connection in _connections)
        {
            Console.WriteLine($"Boards from {connection.Name}");
            connection.Lead();
            while (true)
            {
                Int2[]? boats = connection.GetBoard();
                if (boats is null)
                    break;
                
                var tasks = new Task[_connections.Count];
                
                //Let the others but the original tournament play the game
                bool skipped = false;
                for (int j = 0; j < _connections.Count; j++)
                {
                    var recipient = _connections[j];
                    if (recipient == connection)
                    {skipped = true; continue;}
                    var task = Task.Run(() => recipient.PlayRound(boats));
                    tasks[skipped ? j : j - 1] = task;
                }
                
                //Simulate the local games
                var local = Task.Run(() =>
                {
                    Game game = new(boats, settings);
                    foreach (var competitor in _participants)
                    {
                        int ammOfMoves = game.SimulateGame(competitor.GameStrategy);
                        competitorsScores[competitor] += ammOfMoves;
                    }
                });
                tasks[_connections.Count-1] = local;
                
                await Task.WhenAll(tasks);
            }
        }
        
        Dictionary<(string Participant, string Tournament), int> scores = new();
        foreach (var participant in _participants)
            scores.Add((participant.Name, "Local"), competitorsScores[participant]);

        foreach (var connection in _connections)
        {
            Dictionary<string, int> connectionScores = connection.GetResults();
            foreach (var score in connectionScores)
                scores.Add((score.Key, connection.Name), score.Value);
        }
        
        Console.WriteLine("Local results:");
        DrawResultTable(competitorsScores);
        
        Console.WriteLine("Total results:");
        DrawResultTable(scores);
    }

    public void AddConnection(int port)
    {
        TournamentConnection connection = new(port, IPAddress);
        _connections.Add(connection);
    }

    public void DrawResultTable(Dictionary<(string Participant, string Tournament), int> scores)
    {
        //Final results
        //Draw it as a table with -+| and stuff
        const int nameWidth = 20;
        const int avgWidth = 10;
        const int totalWidth = 20;

        Console.WriteLine("\nTotal amount of moves needed to solve all the opponents' boards:");
        Console.WriteLine($"{"Name",-nameWidth}|{"Tournament",-nameWidth}|{"Total",-totalWidth}|{"Avg",-avgWidth}");
        Console.WriteLine($"{"".PadRight(nameWidth, '-')}+{"".PadRight(nameWidth, '-')}+{
            "".PadRight(totalWidth, '-')}+{"".PadRight(avgWidth, '-')}");
        foreach (var participant in
                 scores.OrderBy(x => x.Value))
        {
            var avg = participant.Value / (double)_gamesPerBoard / (scores.Count-1); // -1 = without himself
            Console.WriteLine(
                $"{participant.Key.Participant,-nameWidth}|{participant.Key.Tournament,-nameWidth}|{
                    participant.Value,-totalWidth}|{avg}");
        }
    }

    public void EndTournament()
    {
        foreach (var connection in _connections)
            connection.Close();
    }
}

public class TournamentConnection
{
    private readonly Socket _client;
    public readonly string Name;

    public TournamentConnection(int port, IPAddress? ipAddress=null, string? name=null)
    {
        if (ipAddress is null)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            ipAddress = ipHost.AddressList[0];
        }

        Console.WriteLine("Opening at " + ipAddress.ToString());
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
        
        Socket socket = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(localEndPoint);
        socket.Listen(10);
        Console.WriteLine(port.ToString() + ": Waiting for connection.");
        _client = socket.Accept();
        Console.WriteLine(port.ToString() + ": Connected.");
        
        name ??= port.ToString();
        Name = name;
    }

    public Dictionary<string, int> GetResults()
    {
        _client.Send("Send your results.<EOM>"u8.ToArray());
        
        byte[] bytes = new Byte[1024];
        string input = "";
 
        while (true)
        {
            int numByte = _client.Receive(bytes);
            input += Encoding.ASCII.GetString(bytes,
                0, numByte);
            if (input.IndexOf("<EOM>") > -1)
                break;
        }

        input = input.Replace("<EOM>", "");
        
        Dictionary<string, int> results = new();
        string[] participants = input.Split(",");
        foreach (string participant in participants)
        {
            string[] parts = participant.Split(":");
            results.Add(parts[0], int.Parse(parts[1]));
        }
        return results;
    }

    public void SendSetting(GameSetting setting)
    {
        _client.Send("Settings.<EOM>"u8.ToArray());
        
        byte[] bytes = new Byte[1024];
        string input = "";
        while (true)
        {
            int numByte = _client.Receive(bytes);
            input += Encoding.ASCII.GetString(bytes,
                0, numByte);
            if (input.IndexOf("Ready.<EOM>") > -1)
                break;
        }

        string settingString = setting.Height.ToString() + '-' + setting.Width.ToString() + '-';
        for (int i = 0; i < setting.BoatCount.Length; i++)
        {
            settingString += setting.BoatCount[i].ToString();
            if (i < setting.BoatCount.Length - 1)
                settingString += ",";
        }
        settingString += "<EOM>";
        _client.Send(Encoding.ASCII.GetBytes(settingString));
        
        input = "";
        while (true)
        {
            int numByte = _client.Receive(bytes);
            input += Encoding.ASCII.GetString(bytes,
                0, numByte);
            if (input.IndexOf("Done.<EOM>") > -1)
                break;
        }
    }

    public void PlayRound(Int2[] boatPositions)
    {
        _client.Send("Play a game.<EOM>"u8.ToArray());
        
        byte[] bytes = new Byte[1024];
        string input = "";
        while (true)
        {
            int numByte = _client.Receive(bytes);
            input += Encoding.ASCII.GetString(bytes,
                0, numByte);
            if (input.IndexOf("Ready.<EOM>") > -1)
                break;
        }

        string boats = "";
        for (int i = 0; i < boatPositions.Length; i++)
        {
            boats += boatPositions[i].X + "," + boatPositions[i].Y;
            if (i < boatPositions.Length - 1)
                boats += "+";
        }
        boats += "<EOM>";
        _client.Send(Encoding.ASCII.GetBytes(boats));
        
        input = "";
        while (true)
        {
            int numByte = _client.Receive(bytes);
            input += Encoding.ASCII.GetString(bytes,
                0, numByte);
            if (input.IndexOf("Done.<EOM>") > -1)
                break;
        }
    }

    public void Lead()
    {
        _client.Send("Lead.<EOM>"u8.ToArray());
    }

    public Int2[]? GetBoard()
    {
        byte[] bytes = new Byte[1024];
        string input = "";
        while (true)
        {
            int numByte = _client.Receive(bytes);
            input += Encoding.ASCII.GetString(bytes,
                0, numByte);
            if (input.IndexOf("<EOM>") > -1)
                break;
        }
        input = input.Replace("<EOM>", "");

        if (input == "Done.")
            return null;
        
        _client.Send("Got it.<EOM>"u8.ToArray());
        
        string[] boatsArr = input.Split("+");
        List<Int2> boatPositions = new();
        foreach (string boat in boatsArr)
        {
            string[] boatArr = boat.Split(",");
            Int2 position = new(int.Parse(boatArr[0]), int.Parse(boatArr[1]));
            boatPositions.Add(position);
        }
        return boatPositions.ToArray();
    }

    public void Close()
    {
        _client.Send("Close!<EOM>"u8.ToArray());
        _client.Close();
    }
}