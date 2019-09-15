using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	public InputField IpInput;
	public InputField PortInput;

    public void HostGame()
	{
		GameManager.Instance.HostGame();
	}

	public void ConnectToGame()
	{
		GameManager.Instance.ConnectToGame(IpInput.text, Int32.Parse(PortInput.text));
	}

	public void ExitGame()
	{
		GameManager.Instance.ExitGame();
	}
}
