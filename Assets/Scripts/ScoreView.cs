using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreView : MonoBehaviour
{
	private Text _text;

    private void Start()
    {
		_text = GetComponent<Text>();
		ScoreManager.Instance.OnScoreUpdate += OnScoreUpdateHandler;
    }

	private void OnDestroy()
	{
		ScoreManager.Instance.OnScoreUpdate -= OnScoreUpdateHandler;
	}

    private void OnScoreUpdateHandler(Player player, int score)
    {
		var localPlayer = NetworkManager.singleton.client.connection.playerControllers.First().gameObject.GetComponent<Player>();
		if (localPlayer != player) return;
		_text.text = score.ToString();
    }
}
