using UnityEngine;

public class CameraTracking : MonoBehaviour
{
	public Transform Target;
	public float allowedOffset = 10f;
	public float smoothing = 3f;
	public Vector3 Offset = new Vector3(0, 20, -9);

    void Update()
    {
		if (Target == null) return;
		transform.position = Vector3.Lerp (transform.position, Target.position + Offset, smoothing * Time.deltaTime);
    }
}
