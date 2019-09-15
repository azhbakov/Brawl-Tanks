using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TeslaShield : NetworkBehaviour
{
	public float Duration = 0f;
	public float Cooldown = 3f;
	public float Radius = 5f;
	private float _lastActivationTime;
	public ParticleSystem Effect;

	public void Activate()
	{
		if (!isServer)
		{
			if (_lastActivationTime + Duration + Cooldown > Time.time) return;
			_lastActivationTime = Time.time;
		}
		CmdActivate();
	}

	[Command]
	private void CmdActivate()
	{
		if (_lastActivationTime + Duration + Cooldown > Time.time) return;
		_lastActivationTime = Time.time;

		// TODO replace overlap to actual sphere for continious effect
		var colliders = Physics.OverlapSphere(transform.position, Radius, LayerMask.GetMask("Projectile"), QueryTriggerInteraction.Ignore);
		foreach (var c in colliders)
		{
			var pc = c.GetComponentInParent<ProjectileController>();
			pc.DisableProjectile();
		}
		RpcDisplayEffects();
	}

	[ClientRpc]
	public void RpcDisplayEffects()
	{
		Effect.Play();
	}
}
