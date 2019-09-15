using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

public class TurretController : NetworkBehaviour
{
	public Rigidbody Turret;
	private Vector3 _targetDirection;
	public float RotationVelocityDeg = 4f;

	private void Awake()
	{
		Assert.IsNotNull(Turret);
	}

	public void LookAt(Vector3 direction)
	{
		_targetDirection = direction;
	}

    void FixedUpdate()
    {
		if (!hasAuthority) return;
		if (_targetDirection == Vector3.zero) return;
		var angleDelta = Quaternion.FromToRotation(Turret.transform.forward, _targetDirection).eulerAngles.y;
		if (angleDelta > 180) angleDelta -= 360f;

		var rotationSpeed = Mathf.Min(Mathf.Abs(angleDelta), RotationVelocityDeg * Time.fixedDeltaTime);
		Turret.MoveRotation(Turret.rotation * Quaternion.Euler(0, rotationSpeed * Mathf.Sign(angleDelta), 0));
    }
}
