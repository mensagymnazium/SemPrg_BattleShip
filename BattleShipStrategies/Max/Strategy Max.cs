using BattleShipEngine;
using System.Data;

namespace BattleShipStrategies.Max
{
    public class Strategy_Max : IGameStrategy
    {
        private GameSetting setting;
        List<Tile> map = new List<Tile>();
        Int2 lastShotPos = new Int2(0, 0);

        public Int2 GetMove()
        {
            lastShotPos = map.OrderByDescending(x => x.hit == false).ThenByDescending(x => x.score).FirstOrDefault().position;
            map.Where(x => x.position == lastShotPos).FirstOrDefault().hit = true;
            /*
                map = map.OrderByDescending(x => x.hit == false).ThenByDescending(x => x.score).ToList();
                var valids = map.Where(x => x.score == map[0].score).ToList();
                lastShotPos = map[rng.Next(valids.Count)].position;
                map.Where(x => x.position == lastShotPos).FirstOrDefault().hit = true;
            */
            return lastShotPos;
        }

        public void RespondHit()
        {
            map.Where(x => x.position == lastShotPos).FirstOrDefault().ship = true;
            foreach (var tile in map.Where(x => x.position == lastShotPos).FirstOrDefault().neighbors)
            {
                tile.score += 10;
            }
        }

        public void RespondSunk()
        {
            EraseNeighbours(lastShotPos);

            void EraseNeighbours(Int2 pos)
            {
                foreach (var tile in map.Where(x => x.position == pos).FirstOrDefault().diagonalNeighbors)
                {
                    tile.hit = true;
                    if (tile.ship && !tile.hit)
                    {
                        EraseNeighbours(tile.position);
                    }
                }
            }
        }

        public void RespondMiss()
        {
            map.Where(x => x.position == lastShotPos).FirstOrDefault().ship = true;
            foreach (var tile in map.Where(x => x.position == lastShotPos).FirstOrDefault().diagonalNeighbors)
            {
                tile.score -= 1;
            }
        }

        public void Start(GameSetting setting)
        {
            map.Clear();
            this.setting = setting;
            for (int y = 0; y < setting.Height; y++)
            {
                for (int x = 0; x < setting.Width; x++)
                {
                    map.Add(new Tile(new Int2(x, y), map));
                }
            }

            foreach (var tile in map)
            {
                tile.FindNeighbours();
            }
        }

        public class Tile
        {
            List<Tile> map = new List<Tile>();
            public Int2 position;
            public bool ship;
            public bool hit;
            public int score;
            public List<Tile> diagonalNeighbors;
            public List<Tile> neighbors;
            public Tile(Int2 position, List<Tile> map)
            {
                this.map = map;
                this.position = position;
                ship = false;
                hit = false;
                score = 0;
                diagonalNeighbors = new List<Tile>();
                neighbors = new List<Tile>();
            }

            public void FindNeighbours()
            {
                Tile neighbor;
                #region assining neighbours
                neighbor = map.Where(a => a.position == position with { X = position.X + 1 }).FirstOrDefault();
                if (neighbor != null)
                {
                    diagonalNeighbors.Add(neighbor);
                    neighbors.Add(neighbor);
                }
                neighbor = map.Where(a => a.position == position with { X = position.X + 1 } with { Y = position.Y + 1 }).FirstOrDefault();
                if (neighbor != null)
                {
                    diagonalNeighbors.Add(neighbor);
                }
                neighbor = map.Where(a => a.position == position with { X = position.X + 1 } with { Y = position.Y - 1 }).FirstOrDefault();
                if (neighbor != null)
                {
                    diagonalNeighbors.Add(neighbor);
                }
                neighbor = map.Where(a => a.position == position with { Y = position.Y + 1 }).FirstOrDefault();
                if (neighbor != null)
                {
                    diagonalNeighbors.Add(neighbor);
                    neighbors.Add(neighbor);
                }
                neighbor = map.Where(a => a.position == position with { Y = position.Y - 1 }).FirstOrDefault();
                if (neighbor != null)
                {
                    diagonalNeighbors.Add(neighbor);
                    neighbors.Add(neighbor);
                }
                neighbor = map.Where(a => a.position == position with { X = position.X - 1 }).FirstOrDefault();
                if (neighbor != null)
                {
                    diagonalNeighbors.Add(neighbor);
                    neighbors.Add(neighbor);
                }
                neighbor = map.Where(a => a.position == position with { X = position.X - 1 } with { Y = position.Y + 1 }).FirstOrDefault();
                if (neighbor != null)
                {
                    diagonalNeighbors.Add(neighbor);
                }
                neighbor = map.Where(a => a.position == position with { X = position.X - 1 } with { Y = position.Y - 1 }).FirstOrDefault();
                if (neighbor != null)
                {
                    diagonalNeighbors.Add(neighbor);
                }
                #endregion
            }
        }
    }
}
