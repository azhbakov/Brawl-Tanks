using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
	public float RespawnTimeSec = 3;

	[SyncVar(hook="TriggerOnScoreChanged")]
	public int Score = 0;

	// Color // TODO separate contract
	public static readonly Color[] AvailableColors = new [] { Color.red, Color.green, Color.blue, Color.yellow, Color.magenta };
	[SyncVar]
	private int _selectedColorInd;

	public PlayerController ModelPrefab;
	private PlayerController _model;

	private void Awake()
	{
		_selectedColorInd = Random.Range(0, AvailableColors.Length);
	}

    public override void OnStartLocalPlayer()
    {
		base.OnStartLocalPlayer();
		CmdSpawnPlayerModel();
    }

	private void TriggerOnScoreChanged(int value)
	{
		ScoreManager.Instance.SetPoints(this, value);
	}

	[Command]
	private void CmdSpawnPlayerModel()
	{
        var spawnPosition = NetworkManager.singleton.GetStartPosition();
		_model = Instantiate<PlayerController>(ModelPrefab, spawnPosition.position, spawnPosition.rotation);
		_model.PlayerId = netId;
		_model.SetColor(_selectedColorInd);
		NetworkServer.SpawnWithClientAuthority(_model.gameObject, gameObject);
	}

	[Server]
	public void OnDeath(Player killer)
	{
		if (killer != null && killer != this)
		{
			killer.Score++;
		}
		var currentModel = _model; // respawn can potentially overwrite model, need to remember current
		Destroy(currentModel.gameObject, RespawnTimeSec);
		Invoke("Respawn", RespawnTimeSec);
	}

	[Server]
    private void Respawn()
    {
        CmdSpawnPlayerModel();
    }
}
