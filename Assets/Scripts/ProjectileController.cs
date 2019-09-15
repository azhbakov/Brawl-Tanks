using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileController : NetworkBehaviour
{
	private Rigidbody _rigidbody;
	private BallisticTrajectoryMovement _movement;
	public GameObject ExplosionPrefab;
	public float ExplosionRadius;
	private Player _owner;
	private bool _hasExploded = false;
	public int Damage = 30;
	public ParticleSystem DisableEffectPrefab;

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
	}

	private void OnCollisionEnter()
	{
		if (!isServer) return;
		if (_hasExploded) return;
		_hasExploded = true;

		var hitObjects = Physics.OverlapSphere(transform.position, ExplosionRadius, LayerMask.GetMask("Player"));
		foreach(var c in hitObjects) 
		{
			var player = c.GetComponentInParent<PlayerController>();
			if (player == null) continue;
			player.TakeDamage(_owner, Damage);
		}
		
		RpcDisplayExplosionEffect();
		Destroy(gameObject, Time.fixedDeltaTime);
	}

	[Server]
    public void Shoot(Player owner, BallisticTrajectory trajectory)
	{
		_owner = owner;
		_movement = new BallisticTrajectoryMovement(trajectory);
		RpcDisplayShootEffect();
	}

	[ClientRpc]
	public void RpcDisplayShootEffect()
	{
		// TODO
	}

	[ClientRpc]
	public void RpcDisplayExplosionEffect()
	{
		var explosion = Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
		Destroy(explosion, explosion.GetComponentInChildren<ParticleSystem>().main.duration);
	}

	public void FixedUpdate()
	{
		if (!isServer) return;
		if (_movement == null) return;
		_movement.Update(Time.fixedDeltaTime); // TODO consider first step
		_rigidbody.velocity = _movement.CurrentVelocity;
	}

	[Server]
	public void DisableProjectile()
	{
		_hasExploded = true;
		RpcDisplayDisableEffect();
		Destroy(gameObject, 0.1f);
	}

	[ClientRpc]
	public void RpcDisplayDisableEffect()
	{
		Instantiate(DisableEffectPrefab, transform.position, transform.rotation);
	}
}

