using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
	public static ScoreManager Instance { get; private set; }
    private Dictionary<Player, int> _score = new Dictionary<Player, int>();
	public ReadOnlyDictionary<Player, int> GetScore() => new ReadOnlyDictionary<Player, int>(_score);

	public event Action<Player, int> OnScoreUpdate;

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	public void Reset()
	{
		_score.Clear();
	}

	public void SetPoints(Player player, int points)
	{
		if (!_score.TryGetValue(player, out var _)) _score.Add(player, 0);
		_score[player] = points;
		OnScoreUpdate?.Invoke(player, points);
	}
}
