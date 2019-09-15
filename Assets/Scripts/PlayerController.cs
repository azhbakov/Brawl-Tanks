using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(ChassisController))]
[RequireComponent(typeof(TurretController))]
[RequireComponent(typeof(CannonController))]
[RequireComponent(typeof(TeslaShield))]
public class PlayerController : NetworkBehaviour
{
	[SyncVar]
	public NetworkInstanceId PlayerId;
	public Player Player => isServer ? 
		NetworkServer.FindLocalObject(PlayerId).GetComponent<Player>() 
		: ClientScene.FindLocalObject(PlayerId).GetComponent<Player>();

	private ChassisController _chassisController;
	private TurretController _turretController;
	private CannonController _cannonController;
	private TeslaShield _teslaShield;

	[SyncVar(hook="SetColor")]
	private int _selectedColorInd;
	public List<Renderer> PartsToApplyColor;

	[Min(1)]
	public int StartingHealth = 100;
	[SyncVar(hook="UpdateHealthBar")] // TODO invert dependency
	private int _currentHealth;
	[SyncVar]
	private bool _isDead = false;
	private Slider _healthBar;

	private bool _isHidden = false;
	private bool _isRevealed = false;

	public float InputMagnitudeForShotCancel = 0.05f;
	
	private void Awake()
	{
		_healthBar = GetComponentInChildren<Slider>();
		_currentHealth = StartingHealth;
		_chassisController = GetComponent<ChassisController>();
		_turretController = GetComponentInChildren<TurretController>();
		_cannonController = GetComponentInChildren<CannonController>();
		_teslaShield = GetComponentInChildren<TeslaShield>();
	}

	private void Start()
	{
		SetColor(_selectedColorInd);
	}

	// PlayerController is not LocalPlayer, but it's under its authority
	public override void OnStartAuthority()
	{
		Camera.main.GetComponent<CameraTracking>().Target = transform;
		GetComponentInChildren<HideoutRevealer>(true).gameObject.SetActive(true);
	}

	public void SetHidden(bool hide) // TODO move to separate contract
	{
		_isHidden = hide;
		if (_isRevealed) Hide(false);
		else Hide(hide);
	}

	private void Hide(bool hide)
	{
		var renderers = GetComponentsInChildren<Renderer>(true);
		var dontHideUi = hasAuthority;
		foreach (var r in renderers) 
		{
			if (dontHideUi && r is LineRenderer) continue;
			r.enabled = !hide;
		}
		if (!dontHideUi) _healthBar.GetComponentsInParent<Canvas>(true).First().gameObject.SetActive(!hide);
	}

	public void SetRevealed(bool reveal)
	{
		_isRevealed = reveal;
		if (reveal) Hide(false);
		else Hide(_isHidden);
	}

	private void Update()
    {
		if (!hasAuthority) return;
		if (_isDead) 
		{
			_cannonController.AimHigh(Vector3.zero);
			return;
		}

        var input = JoystickInput.Left.CurrentJoystickDirection3;
		_chassisController.Move(input);

		// if (_joystickInput.CurrentJoystickDirection3 != Vector3.zero)
		// {
		// }
		// else
		// {
		// 	_turretController.LookAt(transform.forward);
		// }
		_turretController.LookAt(JoystickInput.Right.CurrentJoystickDirection3);
		_cannonController.AimHigh(JoystickInput.Right.CurrentJoystickDirection3);

		if (Input.GetKeyDown(KeyCode.Space) || JoystickInput.Right.Released && JoystickInput.Right.CurrentJoystickDirection3.magnitude > InputMagnitudeForShotCancel)
		{
			_cannonController.Fire();
		}
		if (Input.GetKeyDown(KeyCode.V) || JoystickInput.Right.Pressed)
		{
			_teslaShield.Activate();
		}

		if (Input.GetKeyDown(KeyCode.C))
		{
			TakeDamage(Player, 20);
		}
	}

	private void UpdateHealthBar(int value)
	{
		_healthBar.value = value;
	}

	[Server]
	public void TakeDamage(Player from, int damage)
	{
		_currentHealth -= damage;
		if (_currentHealth < 0) _currentHealth = 0;
		
		UpdateHealthBar(_currentHealth);
		
		if (_currentHealth <= 0 && !_isDead)
		{
			Die(from);
		}
	}

	[Server]
	public void Die(Player killer)
	{
		if (_isDead) return;
		_isDead = true;
		Player.OnDeath(killer);
		RpcPlayDeathEffects();
	}

	[ClientRpc]
	public void RpcPlayDeathEffects()
	{
		_chassisController.Explode();
	}

	public void SetColor(int colorInd)
	{
		_selectedColorInd = colorInd; // on server
		foreach (var part in PartsToApplyColor)
		{
			part.material.color = Player.AvailableColors[_selectedColorInd];
		}
	}
}