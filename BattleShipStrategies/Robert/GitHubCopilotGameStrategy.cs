using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleShipEngine;

namespace BattleShipStrategies.Robert
{
	public class GitHubCopilotGameStrategy : IGameStrategy
	{
		private GameSetting _settings;
		private bool[,] _shots;
		private Queue<Int2> _nextShots;
		private Random _random;

		public void Start(GameSetting setting)
		{
			_settings = setting;
			_shots = new bool[setting.Width, setting.Height];
			_nextShots = new Queue<Int2>();
			_random = new Random();
		}

		public Int2 GetMove()
		{
			Int2 move;

			if (_nextShots.Count > 0)
			{
				move = _nextShots.Dequeue();
			}
			else
			{
				do
				{
					move = new Int2(_random.Next(_settings.Width), _random.Next(_settings.Height));
				}
				while (_shots[move.X, move.Y]);
			}

			_shots[move.X, move.Y] = true;
			return move;
		}

		public void RespondHit()
		{
			var lastMove = GetLastMove();
			AddSurroundingMoves(lastMove);
		}

		public void RespondSunk()
		{
			// Clear the queue when a ship is sunk
			_nextShots.Clear();
		}

		private void AddSurroundingMoves(Int2 lastMove)
		{
			// Add all surrounding moves to the queue
			for (int x = lastMove.X - 1; x <= lastMove.X + 1; x++)
			{
				for (int y = lastMove.Y - 1; y <= lastMove.Y + 1; y++)
				{
					if (x >= 0 && x < _settings.Width && y >= 0 && y < _settings.Height)
					{
						_nextShots.Enqueue(new Int2(x, y));
					}
				}
			}
		}

		private Int2 GetLastMove()
		{
			// Find the last move
			for (int x = 0; x < _settings.Width; x++)
			{
				for (int y = 0; y < _settings.Height; y++)
				{
					if (_shots[x, y])
					{
						return new Int2(x, y);
					}
				}
			}

			throw new Exception("No last move found");
		}

		public void RespondMiss()
		{
			// NOOP
		}
	}
}
