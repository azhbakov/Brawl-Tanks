using UnityEngine;

public struct BallisticTrajectory
{
	// no incapsulation for deserialization
	public Vector3 Start;
	public Vector3 Destination;
	public Vector3 LaunchVector;
	public float Gravity;
	public float ArcHeight;
	public float ETA;

	public BallisticTrajectory(Vector3 from, Vector3 to, float arcHeight, float eta)
	{
		// TODO check if input is correct
		Start = from; Destination = to; ArcHeight = arcHeight; ETA = eta;

		Gravity = -4f * (Start.y - 2 * ArcHeight + Destination.y) / Mathf.Pow(ETA, 2);
		var launchHorizontalVelocity = (Destination - Start).magnitude;
		var launchVerticalVelocity = -(3 * Start.y - 4 * ArcHeight + Destination.y) / ETA;
		
		var horizontalToTarget = Destination - Start;
		horizontalToTarget.y = 0;
		horizontalToTarget.Normalize();
		
		LaunchVector = horizontalToTarget * launchHorizontalVelocity + Vector3.up * launchVerticalVelocity;
	}

	public void Draw(LineRenderer lineRenderer)
	{
		var movement = new BallisticTrajectoryMovement(this);
		var step = 0.05f;
		lineRenderer.positionCount = Mathf.FloorToInt(1/step);

		for (var i = 0f; i <= 1; i += step)
		{
			movement.Update(movement.Trajectory.ETA * step);
			lineRenderer.SetPosition(Mathf.FloorToInt(i/step), movement.CurrentPosition);
		}
	}
}

public class BallisticTrajectoryMovement
{
	public BallisticTrajectory Trajectory { get; }
	public float TimeProgress { get; private set; } = 0f;
	public Vector3 CurrentPosition 
	{ 
		get => Trajectory.Start + (Trajectory.LaunchVector * TimeProgress) + (Vector3.down * Trajectory.Gravity * Mathf.Pow(TimeProgress, 2)) / 2f;
	}
	public Vector3 CurrentVelocity 
	{
		get => Trajectory.LaunchVector + Vector3.down * Trajectory.Gravity * TimeProgress;
	}

	public BallisticTrajectoryMovement(BallisticTrajectory trajectory)
	{
		Trajectory = trajectory;
	}

	public void Update(float deltaTime)
	{
		TimeProgress += deltaTime;
	}
}
