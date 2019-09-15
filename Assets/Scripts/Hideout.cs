using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hideout : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
	{
		var player = collider.gameObject.GetComponentInParent<PlayerController>();
		player?.SetHidden(true);
	}

	private void OnTriggerExit(Collider collider)
	{
		var player = collider.gameObject.GetComponentInParent<PlayerController>();
		player?.SetHidden(false);
	}
}
