using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleShipEngine;

namespace BattleShipStrategies.Robert;

public class ChatGpt2GameStrategy : IGameStrategy
{
	private GameSetting _settings;
	private List<Int2> _potentialTargets;
	private Random _random;
	private Queue<Int2> _nextTargets;
	private Int2? _lastHit;

	public void Start(GameSetting setting)
	{
		_settings = setting;
		_random = new Random();
		_nextTargets = new Queue<Int2>();
		_potentialTargets = new List<Int2>();
		_lastHit = null;

		for (int y = 0; y < _settings.Height; y++)
		{
			for (int x = 0; x < _settings.Width; x++)
			{
				_potentialTargets.Add(new Int2(x, y));
			}
		}
	}

	public Int2 GetMove()
	{
		Int2 move;
		if (_nextTargets.Count > 0)
		{
			move = _nextTargets.Dequeue();
		}
		else
		{
			int index = _random.Next(_potentialTargets.Count);
			move = _potentialTargets[index];
			_potentialTargets.RemoveAt(index);
		}

		_lastHit = move; // Nastavení _lastHit na aktuální tah
		return move;
	}

	public void RespondHit()
	{
		if (_lastHit.HasValue)
		{
			AddSurroundingTargets(_lastHit.Value);
		}
	}

	public void RespondMiss()
	{
		_lastHit = null; // Vynulování _lastHit, pokud byl tah neúspěšný
	}

	public void RespondSunk()
	{
		_nextTargets.Clear();
		_lastHit = null;
	}

	private void AddSurroundingTargets(Int2 lastHit)
	{
		var directions = new[] { (-1, 0), (1, 0), (0, -1), (0, 1) };

		foreach (var (dx, dy) in directions)
		{
			var newX = lastHit.X + dx;
			var newY = lastHit.Y + dy;
			if (newX >= 0 && newX < _settings.Width && newY >= 0 && newY < _settings.Height)
			{
				Int2 newTarget = new Int2(newX, newY);
				if (_potentialTargets.Contains(newTarget))
				{
					_nextTargets.Enqueue(newTarget);
					_potentialTargets.Remove(newTarget);
				}
			}
		}
	}
}
