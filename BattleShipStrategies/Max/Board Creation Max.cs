using BattleShipEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipStrategies.Max
{
    public class Board_Creation_Max : IBoardCreationStrategy
    {
        public Int2[] GetBoatPositions(GameSetting setting)
        {
            Random rng = new Random();
            List<Int2> boats = new List<Int2>();
            List<Int2> restrainedPos = new List<Int2>();

            AddBoat(4);

            AddBoat(3);
            AddBoat(3);

            AddBoat(2);
            AddBoat(2);
            AddBoat(2);

            AddBoat(1);
            AddBoat(1);
            AddBoat(1);    
            AddBoat(1);

            return boats.ToArray();

            void AddBoat(int length)
            {
                int startX = rng.Next(length - 1, 11 - length);
                int startY = rng.Next(length - 1, 11 - length);
                Int2 pos = new Int2(startX, startY);
                int dir = rng.Next(2);
                if (dir == 0)
                {
                    dir = rng.Next(2);
                    for (int i = 0; i < length; i++)
                    {
                        if (dir == 0)
                        {
                            pos = new Int2(startX + i, startY);
                        }
                        else
                        {
                            pos = new Int2(startX - i, startY);
                        }

                        if (restrainedPos.Contains(pos))
                        {
                            AddBoat(length);
                            return;
                        }
                    }
                    for (int i = 0; i < length; i++)
                    {
                        if (dir == 0)
                        {
                            pos = new Int2(startX + i, startY);
                            boats.Add(pos);
                            RestrainNeighbours(pos);
                        }
                        else
                        {
                            pos = new Int2(startX - i, startY);
                            boats.Add(pos);
                            RestrainNeighbours(pos);
                        }
                    }
                }
                else
                {
                    dir = rng.Next(2);
                    for (int i = 0; i < length; i++)
                    {
                        if (dir == 0)
                        {
                            pos = new Int2(startX, startY + i);
                        }
                        else
                        {
                            pos = new Int2(startX, startY - i);
                        }

                        if (restrainedPos.Contains(pos))
                        {
                            AddBoat(length);
                            return;
                        }
                    }
                    for (int i = 0; i < length; i++)
                    {
                        if (dir == 0)
                        {
                            pos = new Int2(startX, startY + i);
                            boats.Add(pos);
                            RestrainNeighbours(pos);
                        }
                        else
                        {
                            pos = new Int2(startX, startY - i);
                            boats.Add(pos);
                            RestrainNeighbours(pos);
                        }
                    }
                }

                void RestrainNeighbours(Int2 pos)
                {
                    restrainedPos.Add(pos);
                    restrainedPos.Add(new Int2(pos.X + 1, pos.Y + 1));
                    restrainedPos.Add(new Int2(pos.X + 1, pos.Y));
                    restrainedPos.Add(new Int2(pos.X + 1, pos.Y - 1));
                    restrainedPos.Add(new Int2(pos.X, pos.Y + 1));
                    restrainedPos.Add(new Int2(pos.X, pos.Y - 1));
                    restrainedPos.Add(new Int2(pos.X - 1, pos.Y + 1));
                    restrainedPos.Add(new Int2(pos.X - 1, pos.Y));
                    restrainedPos.Add(new Int2(pos.X - 1, pos.Y - 1));
                }
            }
        }
    }
}
