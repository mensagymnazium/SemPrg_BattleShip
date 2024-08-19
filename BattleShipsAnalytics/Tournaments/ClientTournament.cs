using System.Net;
using System.Net.Sockets;
using System.Text;
using BattleShipEngine;

namespace BattleShipsAnalytics.Tournaments;

public class ClientTournament : AbstractTournament
{
    private readonly Socket _server;
    private Dictionary<Participant, int> _competitorsScores = new ();
    private GameSetting _setting;

    public ClientTournament(List<Participant> participants, int gamesPerBoard, int port, IPAddress? ipAddress=null)
        : base(participants)
    {
        if (ipAddress is null)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            ipAddress = ipHost.AddressList[0];
        }
        
        IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
        
        _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _server.Connect(endPoint);
        Console.WriteLine($"Connected to {endPoint.Address}:{endPoint.Port}");
        
        foreach (var participant in _participants)
            _competitorsScores.Add(participant, 0);
        
        _gamesPerBoard = gamesPerBoard;
    }

    public void Obey()
    {
        while (true)
        {
            byte[] bytes = new Byte[1024];
            string input = "";
 
            while (true)
            {
                int numByte = _server.Receive(bytes);
                 
                input += Encoding.ASCII.GetString(bytes,
                    0, numByte);
                                            
                if (input.IndexOf("<EOM>") > -1)
                    break;
            }

            input = input.Replace("<EOM>", "");

            switch (input)
            {
                case "Close!":
                {
                    _server.Close();
                    return;
                }
                case "Send your results.":
                {
                    string response = "";
                    for (int i = 0; i < _participants.Count; i++)
                    {
                        response += _participants[i].Name + ':' + _competitorsScores[_participants[i]];
                        if (i != _participants.Count - 1)
                            response += ",";
                    }
                    response += "<EOM>";
                    _server.Send(Encoding.ASCII.GetBytes(response));
                    Console.WriteLine("Local results:");
                    DrawResultTable(_competitorsScores);
                    Console.WriteLine();
                    Console.WriteLine("For total scores see server's results.");
                    break;
                }
                case "Play a game.":
                {
                    _server.Send(Encoding.ASCII.GetBytes("Ready.<EOM>"));
                    
                    string boats = "";
                    while (true)
                    {
                        int numByte = _server.Receive(bytes);
                        boats += Encoding.ASCII.GetString(bytes,
                            0, numByte);
                        if (boats.IndexOf("<EOM>") > -1)
                            break;
                    }
                    boats = boats.Replace("<EOM>", "");
                    
                    string[] boatsArr = boats.Split("+");
                    List<Int2> boatPositions = new();
                    foreach (string boat in boatsArr)
                    {
                        string[] boatArr = boat.Split(",");
                        Int2 position = new(int.Parse(boatArr[0]), int.Parse(boatArr[1]));
                        boatPositions.Add(position);
                    }
                    
                    Game game = new(boatPositions.ToArray(), _setting);
                    foreach (var competitor in _participants)
                        _competitorsScores[competitor] += game.SimulateGame(competitor.GameStrategy);
                    
                    _server.Send(Encoding.ASCII.GetBytes("Done.<EOM>"));
                    break;
                }
                case "Settings.":
                {
                    _server.Send(Encoding.ASCII.GetBytes("Ready.<EOM>"));
                    
                    string settingString = "";
                    while (true)
                    {
                        int numByte = _server.Receive(bytes);
                        settingString += Encoding.ASCII.GetString(bytes,
                            0, numByte);
                        if (settingString.IndexOf("<EOM>") > -1)
                            break;
                    }
                    string[] settingArray = settingString.Replace("<EOM>", "").Split("-");
                    string[] boatsArray = settingArray[2].Split(",");
                    int[] boatCounts = new int[boatsArray.Length];
                    for (int i = 0; i < boatsArray.Length; i++)
                        boatCounts[i] = int.Parse(boatsArray[i]);

                    _setting = new GameSetting(int.Parse(settingArray[0]), int.Parse(settingArray[1]), boatCounts);
                    
                    _server.Send(Encoding.ASCII.GetBytes("Done.<EOM>"));
                    break;
                }
                case "Lead.":
                {
                    Lead();
                    _server.Send(Encoding.ASCII.GetBytes("Done.<EOM>"));
                    break;
                }
            }
        }
    }

    private void Lead()
    {
        var localMoves = new Dictionary<Participant, int>();
        foreach (var competitor in _participants)
            localMoves.Add(competitor, 0);
        
        foreach (var participant in _participants)
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
                Int2[] boatPositions = participant.BoardCreationStrategy.GetBoatPositions(_setting);
                Game game = new(boatPositions, _setting);
                foreach (var competitor in _participants)
                {
                    if (competitor == participant)
                        continue;

                    int ammOfMoves = game.SimulateGame(competitor.GameStrategy);
                    _competitorsScores[competitor] += ammOfMoves;
                    currentTotalMoves[competitor] += ammOfMoves;
                    localMoves[competitor] += ammOfMoves;
                }

                string boats = "";
                for (int j = 0; j < boatPositions.Length; j++)
                {
                    boats += boatPositions[j].X + "," + boatPositions[j].Y;
                    if (j < boatPositions.Length - 1)
                        boats += "+";
                }
                boats += "<EOM>";
                _server.Send(Encoding.ASCII.GetBytes(boats));
                
                byte[] bytes = new Byte[1024];
                string input = "";
                while (true)
                {
                    int numByte = _server.Receive(bytes);
                    input += Encoding.ASCII.GetString(bytes,
                        0, numByte);
                    if (input.IndexOf("Got it.<EOM>") > -1)
                        break;
                }
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
        DrawResultTable(localMoves);
        Console.WriteLine();
    }
}