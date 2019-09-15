using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class JoystickDisplay : MonoBehaviour
{
	public bool IsLeftHand; // for editor display
	public JoystickInput.JoystickHand Hand => IsLeftHand ? JoystickInput.JoystickHand.Left : JoystickInput.JoystickHand.Right;
	public Image Background;
	public Image Foreground;
	private JoystickInput _input;
	private Color _defaultColor;
	public float Offset = 100f;

    // Start is called before the first frame update
    void Awake()
    {
		_input = Hand == JoystickInput.JoystickHand.Left ? JoystickInput.Left : JoystickInput.Right;
		_defaultColor = Foreground.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        Foreground.transform.position = Background.transform.position + Offset * new Vector3(_input.CurrentJoystickDirection2.x, _input.CurrentJoystickDirection2.y);
		Foreground.color = _input.Pressed ? Color.gray : _defaultColor;
    }
}
