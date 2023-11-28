using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipStrategies.Robert
{
	using System;
	using System.Collections.Generic;
	using BattleShipEngine;

	public class ChatGpt1GameStrategy : IGameStrategy
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

		public void RespondMiss()
		{
			// No specific action on miss
		}

		private Int2 GetLastMove()
		{
			for (int x = 0; x < _settings.Width; x++)
				for (int y = 0; y < _settings.Height; y++)
					if (_shots[x, y])
						return new Int2(x, y);

			throw new InvalidOperationException("No moves have been made.");
		}

		private void AddSurroundingMoves(Int2 move)
		{
			var directions = new[] { new Int2(0, 1), new Int2(1, 0), new Int2(0, -1), new Int2(-1, 0) };

			foreach (var dir in directions)
			{
				int newX = move.X + dir.X;
				int newY = move.Y + dir.Y;

				if (newX >= 0 && newX < _settings.Width && newY >= 0 && newY < _settings.Height && !_shots[newX, newY])
				{
					_nextShots.Enqueue(new Int2(newX, newY));
				}
			}
		}
	}
}
