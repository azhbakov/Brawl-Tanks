using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HideoutRevealer : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
	{
		var player = collider.gameObject.GetComponentInParent<PlayerController>();
		player?.SetRevealed(true);
	}

	private void OnTriggerExit(Collider collider)
	{
		var player = collider.gameObject.GetComponentInParent<PlayerController>();
		player?.SetRevealed(false);
	}
}
