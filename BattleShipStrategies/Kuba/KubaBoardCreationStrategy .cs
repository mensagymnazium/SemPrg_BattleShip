using BattleShipEngine;
namespace BattleShipStrategies.Kuba;


public class KubaBoardCreationStrategy : IBoardCreationStrategy
{
    static int unknown = 0;
    static int empty = 1;
    static int boat = 2;
    static int sunkenBoat = 3;

    public Int2[] GetBoatPositions(GameSetting setting)
    {
        int[,] myBattleField = new int[10, 10];

    //    while (!ValidateBattlefield(myBattleField))
     //   {
            myBattleField = new int[10, 10];
            FillWithBoats(myBattleField);
            //Console.WriteLine("trying");
            for (int j = 0; j < myBattleField.GetLength(0); j++)
            {
                for (int i = 0; i < myBattleField.GetLength(1); i++)
                {


                         //   Console.Write(myBattleField[j, i] );


                }
               // Console.WriteLine("");




            }
     //   }

        



        Int2[] positions = new Int2[20];
        int index = 0;
        for (int j = 0; j < myBattleField.GetLength(0); j++)
        {
            for (int i = 0; i < myBattleField.GetLength(1); i++)
            {

                if (myBattleField[j, i] == boat|| myBattleField[j, i] == sunkenBoat)
                {
                    positions[index] = new Int2(i, j);
                    index++;
                    if (index > 19)
                    {
                        {
                            break;
                        }
                        //Console.Write(myBattleField[j, i] + "J");
                    }
                }
               // Console.WriteLine("");
            }



        }
        return positions;
    }

        int[,] FillWithBoats(int[,] field)
        {
            Insert4PieceShip(field);
            AllDiagonalsEmpty(field);
            Insert3PieceShip(field);
            AllDiagonalsEmpty(field);
            Insert3PieceShip(field);
            AllDiagonalsEmpty(field);
            Insert2PieceShip(field);
            AllDiagonalsEmpty(field);
            Insert2PieceShip(field);
            AllDiagonalsEmpty(field);
            Insert2PieceShip(field);
            AllDiagonalsEmpty(field);
            Insert1PieceShip(field);
            AllDiagonalsEmpty(field);
            Insert1PieceShip(field);
            AllDiagonalsEmpty(field);
            Insert1PieceShip(field);
            AllDiagonalsEmpty(field);
            Insert1PieceShip(field);

            return field;
        }

        int[,] AllDiagonalsEmpty(int[,] field)
        {
            foreach (int i in field)
            {
                if (i == boat)
                {

                }
            }
            for (int j = 0; j < field.GetLength(0); j++)
            {
                for (int i = 0; i < field.GetLength(1); i++)
                {
                    if (field[j, i] == boat)
                    {
                        MakeDiagonalEmpty(field, new Int2(i, j));
                        MakeNeighboursEmpty(field, new Int2(i, j));
                    }
                }
            }
            return field;
        }

        int[,] Insert4PieceShip(int[,] field)
        {
            List<Int2> possible = new List<Int2>();

            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i, j] == unknown)
                    {
                        possible.Add(new Int2(j, i));
                    }
                }

            }
            /* foreach (int i in field) ///do a for loop otherwise not working
             {
                 if (i == unknown)
                 {
                     possible.Add(i);
                 }
             }*/
            ShuffleList(possible);
            int freeSpace = 0;
            Int2 startOfBoat = new Int2(0, 0);
            foreach (Int2 i in possible)
            {
                if (OnWhichSideHasMostSpace(field, i).Y >= 3)
                {
                    freeSpace = OnWhichSideHasMostSpace(field, i).X;
                    startOfBoat = i;
                    break;
                }
            }
            CompleteBoat(field, startOfBoat, freeSpace, 3);

            return field;
        }

        int[,] Insert3PieceShip(int[,] field)
        {
            List<Int2> possible = new List<Int2>();

            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i, j] == unknown)
                    {
                        possible.Add(new Int2(j, i));
                    }
                }

            }
            /* foreach (int i in field) ///do a for loop otherwise not working
             {
                 if (i == unknown)
                 {
                     possible.Add(i);
                 }
             }*/
            ShuffleList(possible);
            int freeSpace = 0;
            Int2 startOfBoat = new Int2(0, 0);
            foreach (Int2 i in possible)
            {
                if (OnWhichSideHasMostSpace(field, i).Y >= 2)
                {
                    freeSpace = OnWhichSideHasMostSpace(field, i).X;
                    startOfBoat = i;
                    break;
                }
            }
            CompleteBoat(field, startOfBoat, freeSpace, 2);

            return field;
        }

        int[,] Insert2PieceShip(int[,] field)
        {
            List<Int2> possible = new List<Int2>();

            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i, j] == unknown)
                    {
                        possible.Add(new Int2(j, i));
                    }
                }

            }
            /* foreach (int i in field) ///do a for loop otherwise not working
             {
                 if (i == unknown)
                 {
                     possible.Add(i);
                 }
             }*/
            ShuffleList(possible);
            int freeSpace = 0;
            Int2 startOfBoat = new Int2(0, 0);
            foreach (Int2 i in possible)
            {
                if (OnWhichSideHasMostSpace(field, i).Y >= 1)
                {
                    freeSpace = OnWhichSideHasMostSpace(field, i).X;
                    startOfBoat = i;
                    break;
                }
            }
            CompleteBoat(field, startOfBoat, freeSpace, 1);

            return field;
        }

        int[,] Insert1PieceShip(int[,] field)
        {
            List<Int2> possible = new List<Int2>();

            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i, j] == unknown)
                    {
                        possible.Add(new Int2(j, i));
                    }
                }

            }
            /* foreach (int i in field) ///do a for loop otherwise not working
             {
                 if (i == unknown)
                 {
                     possible.Add(i);
                 }
             }*/
            ShuffleList(possible);
            Int2 final = possible.First();
            field[final.Y, final.X] = boat;
            return field;
        }
        int[,] CompleteBoat(int[,] field, Int2 startingPoint, int rotation, int length)
        {
            field[startingPoint.Y, startingPoint.X] = boat;
            switch (rotation)
            {
                case 1: // right
                    {
                        while (length > 0)
                        {
                            field[startingPoint.Y, startingPoint.X + length] = boat;
                            length--;
                        }
                        return field;
                    }
                case 2: // left
                    {
                        while (length > 0)
                        {
                            field[startingPoint.Y, startingPoint.X - length] = boat;
                            length--;
                        }
                        return field;
                    }
                case 3: // up
                    {
                        while (length > 0)
                        {
                            field[startingPoint.Y - length, startingPoint.X] = boat;
                            length--;
                        }
                        return field;
                    }
                case 4: ///down
                    {
                        while (length > 0)
                        {
                            field[startingPoint.Y + length, startingPoint.X] = boat;
                            length--;
                        }
                        return field;
                    }
            }
            return field;
        }

        Int2 OnWhichSideHasMostSpace(int[,] field, Int2 position) //Int2.X == which side Int2.Y == how much space
        {

            int[] order = new int[4];
            for (int k = 0; k < 4; k++)
            {
                int num = Random.Shared.Next(0, 5);
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
                        {
                            if (TestHowMuchSpaceRight(field, position) > biggestSpace)
                            {
                                direction = 1;
                                biggestSpace = TestHowMuchSpaceRight(field, position);
                            }
                            break;
                        }
                    case 2: //left
                        {
                            if (TestHowMuchSpaceLeft(field, position) > biggestSpace)
                            {
                                direction = 2;
                                biggestSpace = TestHowMuchSpaceLeft(field, position);
                            }
                            break;
                        }
                    case 3: //up
                        {
                            if (TestHowMuchSpaceUp(field, position) > biggestSpace)
                            {
                                direction = 3;
                                biggestSpace = TestHowMuchSpaceUp(field, position);
                            }
                            break;
                        }
                    case 4: //down
                        {
                            if (TestHowMuchSpaceDown(field, position) > biggestSpace)
                            {
                                direction = 4;
                                biggestSpace = TestHowMuchSpaceDown(field, position);
                            }
                            break;
                        }
                }
            }
            return new Int2(direction, biggestSpace);
        }

        static List<Int2> ShuffleList(List<Int2> list)
        {
            int n = list.Count;
            Int2 value = new Int2(0, 0);
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


        void MakeNeighboursEmpty(int[,] field, Int2 location)
        {   //down
            if (location.Y < field.GetLength(0) - 1)
            {
                if (field[location.Y + 1, location.X] == unknown)
                {
                    field[location.Y + 1, location.X] = empty;
                }
                
            }
            //up
            if (location.Y > 0)
            {
                if (field[location.Y - 1, location.X] == unknown)
                {
                    field[location.Y - 1, location.X] = empty;
                }
               

            }
            //right 
            if (location.X < field.GetLength(1) - 1)
            {
                if (field[location.Y, location.X + 1] == unknown)
                {
                    field[location.Y, location.X + 1] = empty;
                }
                
                // field[location.Y, location.X +1] = empty;
            }
            //left
            if (location.X > 0)
            {
                if (field[location.Y, location.X - 1] == unknown)
                {
                    field[location.Y, location.X - 1] = empty;
                }
                
                //field[location.Y , location.X-1] = empty;
            }
        }

        void MakeDiagonalEmpty(int[,] field, Int2 location)
        {
            if (location.Y < field.GetLength(0) - 1 && location.X < field.GetLength(1) - 1 && field[location.Y + 1, location.X + 1] == unknown)
            {
                field[location.Y + 1, location.X + 1] = empty;
            }



            if (location.X > 0 && location.Y < field.GetLength(0) - 1 && field[location.Y + 1, location.X - 1] == unknown)
            {
                field[location.Y + 1, location.X - 1] = empty;
            }

            if (location.Y > 0 && location.X < field.GetLength(1) - 1 && field[location.Y - 1, location.X + 1] == unknown)
            {
                field[location.Y - 1, location.X + 1] = empty;
            }

            if (location.X > 0 && location.Y > 0 && field[location.Y - 1, location.X - 1] == unknown)
            {
                field[location.Y - 1, location.X - 1] = empty;
            }

        }
        int TestHowMuchSpaceRight(int[,] field, Int2 location)
        {
            int amountOfSpace = 0;
            int offset = 1;
            Int2 neighbour = new Int2(location.X, location.Y);

            if (neighbour.X + offset <= field.GetLength(1) - 1 && field[neighbour.Y, neighbour.X + offset] == unknown) //checks one next
            {
                amountOfSpace++;
                offset++;
                if (neighbour.X + offset <= field.GetLength(1) - 1 && field[neighbour.Y, neighbour.X + offset] == unknown)  //checks two next
                {
                    amountOfSpace++;
                    offset++;
                    if (neighbour.X + offset <= field.GetLength(1) - 1 && field[neighbour.Y, neighbour.X + offset] == unknown) //checks three next
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

            if (neighbour.Y + offset <= field.GetLength(0) - 1 && field[neighbour.Y + offset, neighbour.X] == unknown) //checks one next
            {
                amountOfSpace++;
                offset++;
                if (neighbour.Y + offset <= field.GetLength(0) - 1 && field[neighbour.Y + offset, neighbour.X] == unknown)  //checks two next
                {
                    amountOfSpace++;
                    offset++;
                    if (neighbour.Y + offset <= field.GetLength(0) - 1 && field[neighbour.Y + offset, neighbour.X] == unknown) //checks three next
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
                if (neighbour.X - offset >= 0 && field[neighbour.Y, neighbour.X - offset] == unknown)  //checks two next
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

            if (neighbour.Y - offset >= 0 && field[neighbour.Y - offset, neighbour.X] == unknown) //checks one next
            {
                amountOfSpace++;
                offset++;
                if (neighbour.Y - offset >= 0 && field[neighbour.Y - offset, neighbour.X] == unknown)  //checks two next
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
    //static bool ValidateBattlefield(int[,] field)
    //{
    //    var ships = new List<int>();
    //    for (var x = 0; x < 10; x++)
    //        for (var y = 0; y < 10; y++)
    //            if (field[x, y] == 1)
    //            {
    //                var length = 1;
    //                while (x + length < 10 && field[x + length, y] == 1)
    //                    field[x + length++, y] = 0;
    //                while (y + length < 10 && field[x, y + length] == 1)
    //                    field[x, y + length++] = 0;
    //                ships.Add(length);
    //            }
    //    ships.Sort();
    //    return string.Join("", ships) == "1111222334";
    //}
}
  