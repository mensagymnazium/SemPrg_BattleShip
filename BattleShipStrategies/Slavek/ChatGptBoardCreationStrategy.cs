using BattleShipEngine;

namespace BattleShipStrategies.Slavek;

public class ChatGptBoardCreationStrategy : IBoardCreationStrategy
{
    public Int2[] GetBoatPositions(GameSetting setting)
    {
        // Mainly human-written part, originally there only were one setting.
        
        int boardWidth = setting.Width;
        int boardHeight = setting.Height;

        // Define boats
        List<int> boatsList = new List<int>();
        for (int i = 1; i <= setting.BoatCount.Length; i++)
        {
            for (int j = 0; j < setting.BoatCount[i-1]; j++)
                boatsList.Add(i);
        }

        int[] boats = boatsList.ToArray();

        int[,] board = new int[boardWidth, boardHeight];
        
        Int2[] boatPositions = new Int2[boats.Sum()];

        int boatIndex = 0;

        while (true)
        {
            foreach (var boatLength in boats)
            {
                // Changed implementation a bit to make it function
                
                (int x, int y) newBoat = PlaceBoat(board, boatLength);

                Int2 newInt2 = new Int2(newBoat.x, newBoat.y);

                if (newInt2 == new Int2(-1, -1))
                {
                    boatPositions = new Int2[boats.Sum()];
                    boatIndex = 0;
                    break;
                }
                
                boatPositions[boatIndex] = newInt2;
                for (int i = 1; i < boatLength; i++)
                {
                    boatIndex++;
                    boatPositions[boatIndex] = new Int2(newBoat.x+i, newBoat.y);
                }

                boatIndex++;
            }
            
            if (boatIndex == boatPositions.GetLength(0))
            {
                // All boats placed successfully
                return boatPositions;
            }
        }
    }

    static (int x, int y) PlaceBoat(int[,] board, int boatLength)
    {
        Random random = new Random();
        int x, y;
        
        int maxAttempts = board.GetLength(0) * board.GetLength(1) * 2;

        while (true)
        {
            if (--maxAttempts <= 0)
            {
                // Restart boat placement process
                //Console.WriteLine("Restarting boat placement process within PlaceBoat...");
                ResetBoard(board);
                break;
            }
            
            x = random.Next(board.GetLength(0));
            y = random.Next(board.GetLength(1));

            bool validPlacement = true;

            for (int i = 0; i < boatLength; i++)
            {
                if (x + i >= board.GetLength(0) || board[x + i, y] == 1)
                {
                    validPlacement = false;
                    break;
                }

                // Check surrounding positions for adjacency
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int newX = x + i + dx;
                        int newY = y + dy;

                        if (newX >= 0 && newX < board.GetLength(0) &&
                            newY >= 0 && newY < board.GetLength(1) &&
                            board[newX, newY] == 1)
                        {
                            validPlacement = false;
                            break;
                        }
                    }
                }
            }

            if (validPlacement)
            {
                for (int i = 0; i < boatLength; i++)
                {
                    board[x + i, y] = 1;
                }
                return (x, y);
            }
        }
        // If maxAttempts reached, return an invalid position
        return (-1, -1);
    }
    
    static void ResetBoard(int[,] board)
    {
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                board[i, j] = 0;
            }
        }
    }
}