using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorDialog : MonoBehaviour
{
	public Text ErrorText;

	public void SetErrorMessage(string message)
	{
		ErrorText.text = message;
	}

	public void Close()
	{
		Destroy(gameObject);
	}
}
