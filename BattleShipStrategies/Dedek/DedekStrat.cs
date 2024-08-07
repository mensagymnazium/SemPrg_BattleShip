using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipEngine
{
    public class DedekGameStrategy : IGameStrategy
    {
        int scale = 12;
        int[,] guessed;
        int[,] correct;
        int coords = 0;
        int test;
        bool shooting = false;
        int shipsize = 0;
        int x;
        int y;
        int blindguess = 0;

        public Int2 GetMove()
        {
            if(shooting)
            {
                if (correct[x, y - 1] == 1 || guessed[x, y + 1] == 0)
                {
                    return new Int2(x - 1, y);
                }
                else if (correct[x, y - 1] == 1 || correct[x, y - 2] == 1)
                {
                    return new Int2(x - 1, y - 4);
                }
                else if (correct[x, y - 1] == 1)
                {
                    return new Int2(x - 1, y - 3);
                }
                else if (correct[x, y + 1] == 1 || guessed[x, y - 1] == 0)
                {
                    return new Int2(x - 1, y - 2);
                }
                else if (correct[x, y + 1] == 1 || correct[x, y + 2] == 1)
                {
                    return new Int2(x - 1, y + 2);
                }
                else if (correct[x, y + 1] == 1)
                {
                    return new Int2(x - 1, y + 1);
                }
                else if (correct[x - 1, y] == 1 || guessed[x + 1, y] == 0)
                {
                    return new Int2(x, y - 1);
                }
                else if (correct[x - 1, y] == 1 || correct[x - 2, y] == 1)
                {
                    return new Int2(x - 4, y - 1);
                }
                else if (correct[x - 1, y] == 1)
                {
                    return new Int2(x -  3, y - 1);
                }
                else if (correct[x + 1, y] == 1 || guessed[x - 1, y] == 0)
                {
                    return new Int2(x - 2, y - 1);
                }
                else if (correct[x + 1, y] == 1 || correct[x + 2, y] == 1)
                {
                    return new Int2(x + 2, y - 1);
                }
                else if (correct[x + 1, y] == 1)
                {
                    return new Int2(x + 1, y - 1);
                }
                else
                {
                    return new Int2(x - 1, y);
                }
            }
            while (coords + 2 < scale * scale)
            {
                int index = (coords/scale) % 2;
                if (guessed[coords / scale, coords % scale] + index == 0)
                {
                    return new Int2(coords / scale - 1, coords % scale - 1);
                }
                coords = coords + 2;
            }
            for (; blindguess < scale * scale; blindguess++)
            {
                if (guessed[blindguess / scale,blindguess % scale] == 0)
                {
                    return new Int2(blindguess / scale - 1, blindguess % scale - 1);
                }
            }
            return new Int2(x - 1, y - 1);
        }

        public void RespondHit()
        {
            shooting = true;
            shipsize++;
            int x = coords / scale;
            int y = coords % scale;

            guessed[coords / scale, coords % scale] = 1;
            correct[coords / scale, coords % scale] = 1;
        }

        public void RespondMiss()
        {
            guessed[coords / scale, coords % scale] = 1;
        }

        public void RespondSunk()
        {
            shooting = false;
            shipsize = 0;
            for (int i = 0; i < scale * scale; i++)
            {
                if (correct[i / scale, i % scale] == 1)
                {
                    correct[i / scale, i % scale] = 0;

                    guessed[i / scale + 1, i % scale + 1] = 1;
                    guessed[i / scale + 1, i % scale - 1] = 1;
                    guessed[i / scale + 1, i % scale] = 1;
                    guessed[i / scale - 1, i % scale + 1] = 1;
                    guessed[i / scale - 1, i % scale - 1] = 1;
                    guessed[i / scale - 1, i % scale] = 1;
                    guessed[i / scale, i % scale + 1] = 1;
                    guessed[i / scale, i % scale - 1] = 1;

                }
            }
        }

        public void Start(GameSetting setting)
        {
            scale = setting.Width + 2;

            guessed = new int[scale, scale];
            correct = new int[scale, scale];

            for (int i = 0; i < scale * scale; i++)
            {
                if (i / scale == 0 || i / scale == scale - 1 || i % scale == 0 || i % scale == scale - 1)
                {
                    guessed[i / scale, i % scale] = 1;
                }
                else
                {
                    guessed[i / scale, i % scale] = 0;
                }
                
                correct[i / scale, i % scale] = 0;
            }
            test = 0;
            coords = 0;
        }
    }
}
