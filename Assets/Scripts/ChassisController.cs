using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class ChassisController : NetworkBehaviour
{
	private Rigidbody _rigidbody;
	public Transform Chassis;
	public float MaxForwardSpeed = 10f;
	public float MaxBackwardSpeed = 3f;
	public float Acceleration = 4f;
	public float MaxRotationAngularVelocity = 4f;
	private Vector3 _inputDirection;
	public float BackwardOffsetDeg = 30;

	private Animator _animator;

    private void Awake()
    {
		_animator = Chassis.GetComponent<Animator>();
		_rigidbody = GetComponent<Rigidbody>();
    }


    public void Move(Vector3 direction)
	{
		_inputDirection = direction;
	}

	public void Explode()
	{
		_animator.SetBool("OnFire", true);
	}

	public void FixedUpdate()
	{
		var isMoving = _rigidbody.velocity.magnitude > 0.001f;
		if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && isMoving)
		{
			_animator.SetBool("IsMoving", true);
		}
		else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Moving") && !isMoving)
		{
			_animator.SetBool("IsMoving", false);
		}

		if (!hasAuthority) return;

		var angleDelta = Quaternion.FromToRotation(transform.forward, _inputDirection).eulerAngles.y;
		
		var isBackwardDirection = angleDelta > (90f+BackwardOffsetDeg) && angleDelta < (270f-BackwardOffsetDeg);
		if (isBackwardDirection) angleDelta -= 180; // don't U-turn if input is backward enough

		var rotationAngle = angleDelta > 180 ? angleDelta - 360f : angleDelta;

		var rotationSpeed = MaxRotationAngularVelocity * _inputDirection.magnitude;
		if (rotationSpeed * Time.fixedDeltaTime > Mathf.Abs(rotationAngle)) rotationSpeed = rotationAngle;
		_rigidbody.angularVelocity = new Vector3(0, Mathf.Sign(rotationAngle) * Mathf.Deg2Rad * rotationSpeed, 0);

		var speed = Mathf.Cos(Mathf.Deg2Rad * Mathf.Abs(rotationAngle)) * _inputDirection.magnitude * (isBackwardDirection? MaxBackwardSpeed : MaxForwardSpeed);
		_rigidbody.velocity = transform.forward * (isBackwardDirection ? -1 : 1 ) * speed;
	}
}
