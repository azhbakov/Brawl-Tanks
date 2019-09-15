using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(MapGenerator))]
[RequireComponent(typeof(UIManager))]
[RequireComponent(typeof(MyNetworkManager))]
[RequireComponent(typeof(ScoreManager))]
public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public Player LocalPlayer;

	private MapGenerator _mapGenerator;
	private UIManager _uiManager;
	private MyNetworkManager _networkManager;
	private ScoreManager _scoreManager;

	private bool _isHost = false;

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);

		_mapGenerator = GetComponent<MapGenerator>();
		_uiManager = GetComponent<UIManager>();
		_networkManager = GetComponent<MyNetworkManager>();
		_scoreManager = GetComponent<ScoreManager>();
	}

	private void Start()
	{
		_uiManager.SetMenuMode();
	}
	
    public void HostGame()
    {
		try
		{
			_networkManager.StartHost(); // TODO more error handling
			_isHost = true;
			_mapGenerator.GenerateMap();
			_uiManager.SetGameMode();
		} 
		catch { /* TODO display error message */ }
    }

	public void ConnectToGame(string ip, int port)
    {
		try
		{
			_networkManager.networkAddress = ip;
			_networkManager.networkPort = port;
			var client = _networkManager.StartClient(); // TODO more error handling
			_uiManager.SetGameMode();
		}
		catch { /* TODO display error message */ }
    }

	public void OnError(string error)
	{
		Leave();
		_uiManager.DisplayError(error);
	}

	public void ExitGame()
	{
		Leave();
		Application.Quit();
	}

	public void Leave()
	{
		if (_isHost) _networkManager.StopHost();
		else _networkManager.StopClient();
		_uiManager.SetMenuMode();
		_scoreManager.Reset();
	}

	public void OnDisconnect()
	{
		_uiManager.SetMenuMode();
		_scoreManager.Reset();
	}
}
