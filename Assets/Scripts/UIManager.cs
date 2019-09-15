using UnityEngine;

public class UIManager : MonoBehaviour
{
	public GameObject MenuUIPrefab;
	public GameObject InGameUIPrefab;
	public ErrorDialog ErrorMessagePrefab;

	private GameObject _currentUi;

    public void SetMenuMode() => SetUiMode(MenuUIPrefab);

	public void SetGameMode() => SetUiMode(InGameUIPrefab);

	public void DisplayError(string message) 
	{
		Instantiate<ErrorDialog>(ErrorMessagePrefab, _currentUi.transform)
			.SetErrorMessage(message);
	}

	private void SetUiMode(GameObject uiPrefab)
	{
		if (_currentUi != null) Destroy(_currentUi);
		_currentUi = Instantiate(uiPrefab);
		_currentUi.transform.SetParent(transform);
	}
}
