using BattleShipEngine;

namespace BattleShipStrategies.MartinF;

public class MartinBoardCreationStrategy : IBoardCreationStrategy
{
    private readonly List<Int2> _impossibleSpots;

    public Int2[] GetBoatPositions(GameSetting setting)
    {
    retryAll:

        var attempts = 0; //For checking if we missed a boat too many times (we probably cant fit it on the board anymore)
        var boatPositions = new List<Int2>();

        for (int i = setting.BoatCount.Length - 1; i >= 0; i--)
        {
            var boatSize = i + 1;
            var boatCount = setting.BoatCount[i];

            //Place each boat
            for (int bcIndex = 0; bcIndex < boatCount; bcIndex++)
            {
            retry:
                var boatDir = Random.Shared.Next(0, 2) == 0 ? new Int2(1, 0) : new Int2(0, -1); //Right or down
                //var boatStartIndex = Random.Shared.Next(0, _impossibleSpots.Count);
                var boatStart =
                    boatDir == new Int2(1, 0)
                        //Horizontal boat, create `size` from left,right edges
                        ? new Int2(Random.Shared.Next(boatSize, setting.Width - boatSize), Random.Shared.Next(0, setting.Height))
                        //Vertical boat, create `size` from top,bottom edges
                        : new Int2(Random.Shared.Next(0, setting.Width), Random.Shared.Next(boatSize, setting.Height - boatSize));

                var boatPieces = new List<Int2>(boatSize);
                for (int bPIndex = 0; bPIndex < boatSize; bPIndex++)
                {
                    var boatPiece = boatStart + new Int2(boatDir.X * bPIndex, boatDir.Y * bPIndex);
                    if (_impossibleSpots.Contains(boatPiece))
                    {
                        //If we placed a piece in an impossible spot, try again
                        boatPieces.Clear();
                        attempts++;

                        //If we tried too many times, start all over...
                        if (attempts > 333)
                        {
                            _impossibleSpots.Clear();
                            goto retryAll;
                        }
                        goto retry;
                    }
                    boatPieces.Add(boatPiece);
                }

                //Remove spots this boat makes impossible
                var boatOutline = MartinF.MartinStrategy.Extensions.GetOutline(boatPieces);
                _impossibleSpots.AddRange(boatOutline);
                _impossibleSpots.AddRange(boatPieces);

                //Add boat to return list
                boatPositions.AddRange(boatPieces);
            }


        }

        var res = boatPositions.ToArray();
        //PrintBoard(res, setting.Width, setting.Height);
        return res;
    }

    private void PrintBoard(Int2[] boatPositions, int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var pos = new Int2(x, y);
                Console.Write(boatPositions.Contains(pos) ? "B " : ". ");
            }

            Console.WriteLine();
        }
    }

    public MartinBoardCreationStrategy()
    {
        _impossibleSpots = new(100);
    }
}

