using BattleShipEngine;
using System.Linq;


namespace BattleShipStrategies.Kuba
{
    public class KubaStrategie : IGameStrategy
    {
        int[,] opponentsField = new int[10, 10];
        Int2 lastMove;
        static int unknown = 0;
        static int empty = 1;
        static int boat = 2;
        static int sunkenBoat = 3;
        
        Int2 GetNextMove()
        { /*
            for (int j = 0; j < 10; j++)
            {


                for (int i = 0; i < 10; i++)
                {
                    Console.Error.Write(opponentsField[j, i]);
                    
                }
                Console.Error.WriteLine();
                
            }
            Thread.Sleep(100);
            Console.Error.WriteLine();
            Console.Error.WriteLine();*/

            // return new Int2(Random.Shared.Next(0, 10), Random.Shared.Next(0, 10));
            // Tuple(Int2, int, int, int, int) 
            //List<Int2> possibleMoves = new List<Int2>();
            // List<Tuple<Int2, int, int>> possibleMoves = new List<Tuple<Int2, int, int>>();
            // Dictionary<Int2, int> possibleMovesAndScores = new Dictionary<Int2, int>();
            //for unknown cells
            Int2 bestInt2 = new Int2(0,0);
            int bestScore = 0;
            List<Tuple<Int2, int>> allUnknown = new List<Tuple<Int2, int>>();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {   if (opponentsField[i, j] == boat)
                    {
                        if (TestHowMuchSpaceRight(opponentsField, new Int2(j, i)) == 0 && TestHowMuchSpaceLeft(opponentsField, new Int2(j, i)) == 0 && TestHowMuchSpaceUp(opponentsField, new Int2(j, i)) == 0 && TestHowMuchSpaceDown(opponentsField, new Int2(j, i)) == 0)
                        {
                            continue;
                        }
                        int[] order = new int[4];
                        for (int k = 0; k < 4; k++)
                        {   int num = Random.Shared.Next(0, 5);
                            while (order.Contains(num))
                            {
                                num = Random.Shared.Next(0, 5);
                            }
                            order[k] = num;
                            
                        }
                        int biggestSpace = 0;
                        int direction = 0;
                        
                        foreach (int num in order)
                        {
                            switch (num)
                            {
                                case 1: //right
                                    {   if (TestHowMuchSpaceRight(opponentsField, new Int2 (j,i))>biggestSpace)
                                        {
                                            direction = 1;
                                            biggestSpace = TestHowMuchSpaceRight(opponentsField, new Int2(j, i));
                                        }
                                        break;
                                    }
                                case 2: //left
                                    {
                                        if (TestHowMuchSpaceLeft(opponentsField, new Int2(j, i)) > biggestSpace)
                                        {
                                            direction = 2;
                                            biggestSpace = TestHowMuchSpaceLeft(opponentsField, new Int2(j, i));
                                        }
                                        break;
                                    }
                                case 3: //up
                                    {
                                        if (TestHowMuchSpaceUp(opponentsField, new Int2(j, i)) > biggestSpace)
                                        {
                                            direction = 3;
                                            biggestSpace = TestHowMuchSpaceUp(opponentsField, new Int2(j, i));
                                        }
                                        break;
                                    }
                                case 4: //down
                                    {
                                        if (TestHowMuchSpaceDown(opponentsField, new Int2(j, i)) > biggestSpace)
                                        {
                                            direction = 4;
                                            biggestSpace = TestHowMuchSpaceDown(opponentsField, new Int2(j, i));
                                        }
                                        break;
                                    }
                            }
                        }
                        switch (direction)
                        {
                            case 1: //right
                                {
                                   return new Int2 (j+1, i);
                                    break;
                                }
                            case 2: //left
                                {
                                    return new Int2(j -1, i);
                                    break;
                                }
                            case 3: //up
                                {
                                    return new Int2(j , i-1);
                                    break;
                                }
                            case 4: //down
                                {
                                    return new Int2(j , i+1);
                                    break;
                                }
                        }



                    }

                    
                    if (opponentsField[i,j] == unknown)
                    {
                            Int2 position = new Int2(j,i); //
                            int right = TestHowMuchSpaceRight(opponentsField, position);
                        //Console.WriteLine("right " + right);
                        int left = TestHowMuchSpaceLeft(opponentsField, position);
                        //Console.WriteLine(  "left" + left);
                        int up = TestHowMuchSpaceUp(opponentsField, position);
                        //Console.WriteLine("up" + up);
                        int down = TestHowMuchSpaceDown(opponentsField, position);
                        //Console.WriteLine("down" + down);
                        int oneInAllDirectiones = 0; // 0 > nothing | 1> one space in opposite sides | 2> one in all direstiones
                        int twoInAllDirectiones = 0;
                        int score = 0;
                        int score2 = 0;
                            if (right >0 && left >0&& up>0&&down>0) //all four sides
                        {
                            oneInAllDirectiones = 2;
                            score += 7;
                            score2 += 5;
                            if (right > 1 && left > 1 && up > 1 && down > 1)
                            {
                                twoInAllDirectiones = 2;
                                score += 8;
                                score2 += 8;
                                if (right > 2 && left > 2 && up > 2 && down > 2)
                                {
                                    twoInAllDirectiones = 2;
                                    score += 9;
                                    score2 += 9;
                                }
                            }
                        }
                            if (left >0 && right >0 || up >0 && down >0) // only two sides
                        {
                            oneInAllDirectiones = 1;
                            score += 4;
                            score2 += 3;
                            if (left > 1 && right > 1|| up > 1 && down > 1)
                            {
                                twoInAllDirectiones = 1;
                                score += 5;
                                score2 += 6;
                                if (left > 2 && right > 2 || up > 2 && down > 2)
                                {
                                    twoInAllDirectiones = 1;
                                    score += 6;
                                    score2 += 7;
                                }
                            }
                        }
                        if (left > 0 || right > 0 || up > 0|| down > 0) // only one side
                        {
                            oneInAllDirectiones = 1;
                            score += 1;
                            score2 += 1;
                            if (left > 1 || right > 1 || up > 1 || down > 1)
                            {
                                twoInAllDirectiones = 1;
                                score += 2;
                                score2 += 2;
                                if (left > 2 || right > 2 || up > 2 || down > 2)
                                {
                                    twoInAllDirectiones = 1;
                                    score += 3;
                                    score2 += 4;
                                }
                            }
                        }

                        allUnknown.Add(new Tuple<Int2, int>(position, score2));
                        // Console.WriteLine(score);
                        /* if (score >= bestScore)
                         {
                             bestInt2 = position;
                             bestScore = score;
                         }*/

                        //possibleMovesAndScores.Add(position, score);
                        //possibleMoves.Add(new Tuple<Int2, int, int>(position,oneInAllDirectiones, twoInAllDirectiones));
                    }

                }
            }
            ShuffleList(allUnknown);
            foreach (Tuple<Int2, int> cell in allUnknown)
            {
                if (cell.Item2 >= bestScore)
                {
                    bestInt2 = cell.Item1;
                    bestScore = cell.Item2;
                }
            }
            // return possibleMoves.OrderBy(x => x.Item3).ThenBy(x=>x.Item2).First().Item1;
            //Int2 best = possibleMovesAndScores.OrderBy(x => x.Value).First().Key;

            return bestInt2;

        }
        List<Tuple<Int2, int>> ShuffleList(List<Tuple<Int2, int>> list)
        {
            int n = list.Count;
            Tuple<Int2, int> value;
            while (n > 1)
            {
                n--;
                int k = Random.Shared.Next(n + 1);
                value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }
        public Int2 GetMove()
        {
            

                Int2 move = GetNextMove();
                
                    lastMove = move;
                    return move;
                
                
                




        }

        public void RespondHit()
        {
            opponentsField[lastMove.Y, lastMove.X] = boat;
            MakeDiagonalEmpty(opponentsField, lastMove);
            
            
        }

        public void RespondMiss()
        { 
            opponentsField[lastMove.Y, lastMove.X] = empty;
            
            
        }

        public void RespondSunk()
        {
            opponentsField[lastMove.Y, lastMove.X] = sunkenBoat;
            MakeNeighboursEmpty(opponentsField, lastMove);
        }

        public void Start(GameSetting setting)
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    opponentsField[i, j] = 0;
                }
            }
        }

        public void MakeNeighboursEmpty (int[,] field, Int2 location)
        {   //down
            if (location.Y < field.GetLength(0) - 1 )
            {   
                if (field[location.Y + 1, location.X] == unknown)
                {
                    field[location.Y + 1, location.X] = empty;
                }
                else if (field[location.Y + 1, location.X] == boat)
                {
                    field[location.Y + 1, location.X] = sunkenBoat;
                    MakeNeighboursEmpty(field, new Int2(location.X,location.Y + 1));
                }
            }
            //up
            if (location.Y > 0)
            {
                if (field[location.Y - 1, location.X] == unknown)
                {
                    field[location.Y - 1, location.X] = empty;
                }
                else if (field[location.Y - 1, location.X] == boat)
                {
                    field[location.Y - 1, location.X] = sunkenBoat;
                    MakeNeighboursEmpty(field, new Int2( location.X, location.Y - 1));
                }
                
            }
            //right 
            if (location.X < field.GetLength(1) - 1)
            {
                if (field[location.Y, location.X+1] == unknown)
                {
                    field[location.Y , location.X+1] = empty;
                }
                else if (field[location.Y , location.X+1] == boat)
                {
                    field[location.Y, location.X + 1] = sunkenBoat;
                    MakeNeighboursEmpty(field, new Int2(location.X + 1, location.Y));
                }
               // field[location.Y, location.X +1] = empty;
            }
            //left
            if (location.X > 0)
            {
                if (field[location.Y, location.X-1] == unknown)
                {
                    field[location.Y , location.X-1] = empty;
                }
                else if (field[location.Y , location.X-1] == boat)
                {
                    field[location.Y, location.X - 1] = sunkenBoat;
                    MakeNeighboursEmpty(field, new Int2(location.X - 1, location.Y));
                }
                //field[location.Y , location.X-1] = empty;
            }
        }

        public void MakeDiagonalEmpty(int[,] field, Int2 location)
        {
            if (location.Y < field.GetLength(0)-1 && location.X < field.GetLength(1)-1&& field[location.Y + 1, location.X + 1] == unknown)
            {
                field[location.Y + 1, location.X + 1] = empty;
            }



            if (location.X > 0 && location.Y < field.GetLength(0)-1&& field[location.Y + 1, location.X - 1]==unknown)
            {
                field[location.Y + 1, location.X - 1] = empty;
            }

            if (location.Y > 0 && location.X < field.GetLength(1)-1&& field[location.Y - 1, location.X + 1]==unknown)
            {
                field[location.Y - 1, location.X + 1] = empty;
            }

            if (location.X > 0 && location.Y > 0&& field[location.Y - 1, location.X - 1]==unknown)
            {
                field[location.Y - 1, location.X - 1] = empty;
            }

        }
        int TestHowMuchSpaceRight(int[,] field, Int2 location)
        {
            int amountOfSpace = 0;
            int offset = 1;
            Int2 neighbour = new Int2(location.X, location.Y);

            if (neighbour.X + offset <= field.GetLength(1)-1 && field[neighbour.Y, neighbour.X+offset] == unknown) //checks one next
            {   
                amountOfSpace++;
                offset++;
                if (neighbour.X + offset <= field.GetLength(1)-1 && field[neighbour.Y, neighbour.X + offset] == unknown)  //checks two next
                {
                    amountOfSpace++;
                    offset++;
                    if (neighbour.X + offset <= field.GetLength(1)-1 && field[neighbour.Y, neighbour.X + offset] == unknown) //checks three next
                    {
                        amountOfSpace++;
                    }
                }
            }
            return amountOfSpace;
        }

        int TestHowMuchSpaceDown(int[,] field, Int2 location)
        {
            int amountOfSpace = 0;
            int offset = 1;
            Int2 neighbour = new Int2(location.X, location.Y);

            if (neighbour.Y + offset<= field.GetLength(0)-1 && field[neighbour.Y+offset, neighbour.X] == unknown) //checks one next
            {
                amountOfSpace++;
                offset++;
                if (neighbour.Y + offset<= field.GetLength(0)-1 && field[neighbour.Y+offset, neighbour.X] == unknown)  //checks two next
                {
                    amountOfSpace++;
                    offset++;
                    if (neighbour.Y + offset <= field.GetLength(0)-1 && field[neighbour.Y+offset, neighbour.X] == unknown) //checks three next
                    {
                        amountOfSpace++;
                    }
                }
            }
            return amountOfSpace;
        }

        int TestHowMuchSpaceLeft(int[,] field, Int2 location)
        {
            int amountOfSpace = 0;
            int offset = 1;
            Int2 neighbour = new Int2(location.X, location.Y);

            if (neighbour.X - offset >= 0 && field[neighbour.Y, neighbour.X - offset] == unknown) //checks one next
            {
                amountOfSpace++;
                offset++;
                if (neighbour.X - offset>= 0 && field[neighbour.Y, neighbour.X - offset] == unknown)  //checks two next
                {
                    amountOfSpace++;
                    offset++;
                    if (neighbour.X - offset >= 0 && field[neighbour.Y, neighbour.X - offset] == unknown) //checks three next
                    {
                        amountOfSpace++;
                    }
                }
            }
            return amountOfSpace;
        }
        int TestHowMuchSpaceUp(int[,] field, Int2 location)
        {
            int amountOfSpace = 0;
            int offset = 1;
            Int2 neighbour = new Int2(location.X, location.Y);

            if (neighbour.Y - offset >= 0 && field[neighbour.Y - offset, neighbour.X ] == unknown) //checks one next
            {
                amountOfSpace++;
                offset++;
                if (neighbour.Y - offset>= 0 && field[neighbour.Y - offset, neighbour.X] == unknown)  //checks two next
                {
                    amountOfSpace++;
                    offset++;
                    if (neighbour.Y - offset >= 0 && field[neighbour.Y - offset, neighbour.X] == unknown) //checks three next
                    {
                        amountOfSpace++;
                    }
                }
            }
            return amountOfSpace;
        }
    }
}
