using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

public class CannonController : NetworkBehaviour
{
	public LineRenderer LineRenderer;
	public Rigidbody Cannon;
	private PlayerController _player;
	private Vector3 _currentInputDirection;

	public float MaxRange = 30f;
	public float ArcHeight = 3f;
	public float ProjectileETA = 1f;
	public BallisticTrajectory _inputTrajectory;

	private Vector3 _target { 
		get 
		{
			var r = Cannon.position + Cannon.transform.forward * MaxRange * _currentInputDirection.magnitude;
			r.y = 0;
			return r;
		}
	}
	private Vector3 _actualTarget => Muzzle.position + Muzzle.forward * MaxRange * _currentInputDirection.magnitude;

	public Transform Muzzle;
	public ProjectileController ProjectilePrefab;

	public float FireDelaySec = 1f;
	private float _lastShotTime;
	private bool _canShoot => Time.time - _lastShotTime > FireDelaySec;
	private Color _defaultLineColor;
	
	private void Awake()
	{
		Assert.IsNotNull(Cannon);
		Assert.IsNotNull(Muzzle);
		Assert.IsNotNull(ProjectilePrefab);
		LineRenderer = GetComponentInChildren<LineRenderer>();
		Assert.IsNotNull(LineRenderer);
		_player = GetComponentInParent<PlayerController>();
		Assert.IsNotNull(_player);
	}

	public void AimHigh(Vector3 inputDirection)
	{
		_currentInputDirection = inputDirection;

		_inputTrajectory = new BallisticTrajectory(transform.position, _target, ArcHeight, ProjectileETA);
	}

	private void Update()
    {
		if (!hasAuthority) return;
		if (_currentInputDirection == Vector3.zero)
		{
			LineRenderer.gameObject.SetActive(false);
			return;
		}

		var lineColor = _canShoot ? Color.white : Color.gray;
		LineRenderer.SetColors(lineColor, lineColor);

		LineRenderer.gameObject.SetActive(true);
		_inputTrajectory.Draw(LineRenderer);
    }

	public void Fire()
	{
		if (!_canShoot) return;
		CmdFire(_target);
	}

	[Command]
	private void CmdFire(Vector3 target)
	{
		if (!_canShoot) return;
		var newProjectile = Instantiate(ProjectilePrefab, Muzzle.position, Muzzle.rotation);
		Destroy(newProjectile, ProjectileETA + 5);
		NetworkServer.Spawn(newProjectile.gameObject);

		var trajectory = new BallisticTrajectory(Muzzle.position, target, ArcHeight, ProjectileETA);
		newProjectile.Shoot(_player.Player, trajectory);
		_lastShotTime = Time.time;
	}
}
