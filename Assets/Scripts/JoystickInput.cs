using UnityEngine;

public class JoystickInput : MonoBehaviour
{
	public RectTransform JoystickArea;
	public enum JoystickHand { Left, Right }
	public bool IsLeftHand; // for editor display
	public JoystickHand Hand => IsLeftHand ? JoystickHand.Left : JoystickHand.Right;
	public static JoystickInput Left;
	public static JoystickInput Right;
	public Vector2 CurrentJoystickDirection2;
	public Vector3 CurrentJoystickDirection3 => new Vector3(CurrentJoystickDirection2.x, 0, CurrentJoystickDirection2.y);
	private Vector3 _touchStart;
	private Vector3 _touchCurrent;
	private bool _isTouching;
	private int _touchId;
	public float Radius = 100f;
	public bool Released = false;
	
	private float _touchStartTime;
	public float PressThreshold = 0.1f;
	public bool Pressed;

	private void Awake()
	{
		if (Hand == JoystickHand.Left)
		{
			if (Left == null) Left = this;
			else Destroy(gameObject);
		}
		else
		{
			if (Right == null) Right = this;
			else Destroy(gameObject);
		}
	}

	private void Update()
	{
		#if (UNITY_ANDROID || UNITY_IOS || UNITY_WP8 || UNITY_WP8_1) && !UNITY_EDITOR
		ProcessTouchInput()
		#else
		if (Hand == JoystickHand.Left) 
		{
			ProcessMouseInput();
			if (!_isTouching) ProcessKeyboardInput();
		}
		else ProcessMouseInput();
		#endif
	}

	private void ProcessKeyboardInput()
	{
		CurrentJoystickDirection2 = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		if (CurrentJoystickDirection2.magnitude > 1f) CurrentJoystickDirection2.Normalize();
		// TODO add keyboard released event
	}

	private void ProcessMouseInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (RectTransformUtility.RectangleContainsScreenPoint(JoystickArea, Input.mousePosition))
			{
				_touchStartTime = Time.time;
				_isTouching = true;
				_touchStart = Input.mousePosition;
			}
		}
		if (_isTouching && (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)))
		{
			if (Input.GetMouseButtonUp(0)) 
			{
				_isTouching = false;
				Released = true;
				if (Time.time - _touchStartTime < PressThreshold) Pressed = true;
			}
			_touchCurrent = Input.mousePosition;
			var inputVector = (_touchCurrent - _touchStart);
			CurrentJoystickDirection2 = inputVector.normalized * Mathf.Min(1f, inputVector.magnitude/Radius);
		}
		else
		{
			_isTouching = false;
			Released = false;
			Pressed = false;
			CurrentJoystickDirection2 = Vector2.zero;
		}
	}

	private void ProcessTouchInput()
	{
        for (var i = 0; i < Input.touchCount; i++)
        {
            var touch = Input.GetTouch(i);

            if (touch.phase == TouchPhase.Began 
				&& RectTransformUtility.RectangleContainsScreenPoint(JoystickArea, touch.position)
				&& !_isTouching)
            {
				_touchStartTime = Time.time;
				_isTouching = true;
				_touchId = touch.fingerId;
				_touchStart = touch.position;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                if(_isTouching && _touchId == touch.fingerId)
                {
                    _touchCurrent = touch.position;
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                if(_isTouching && _touchId == touch.fingerId)
                {
					_touchCurrent = touch.position;
                   _isTouching = false;
					Released = true;
					if (Time.time - _touchStartTime < PressThreshold) Pressed = true;
                }
            }
			if (touch.phase == TouchPhase.Canceled)
			{
				_isTouching = false;
				Released = false;
				Pressed = false;
			}
        }

		if (_isTouching || Released)
		{
			var inputVector = (_touchCurrent - _touchStart);
			CurrentJoystickDirection2 = inputVector.normalized * Mathf.Min(1f, inputVector.magnitude/Radius);
		}
		else 
		{
			CurrentJoystickDirection2 = Vector2.zero;
			Released = false;
		}
	}
}