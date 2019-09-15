using UnityEngine.Networking;

public class MyNetworkManager : NetworkManager
{
	public override void OnClientDisconnect(NetworkConnection conn)
	{
		StopClient();
		GameManager.Instance.OnDisconnect();
	}

	public override void OnClientError(NetworkConnection conn, int errorCode)
	{
		var message = ((NetworkError)errorCode).ToString();
		GameManager.Instance.OnError(message);
	}

	public override void OnServerError(NetworkConnection conn, int errorCode)
	{
		var message = ((NetworkError)errorCode).ToString();
		GameManager.Instance.OnError(message);
	}
}