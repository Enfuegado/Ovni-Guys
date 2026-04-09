using UnityEngine;
using TMPro;

public class GameManagerHTTP : MonoBehaviour
{
    [Header("Dependencies")]
    public ApiClient apiClient;
    public SpawnManager spawnManager;
    public GameObject[] playerPrefabs;
    public TextMeshProUGUI statusText;

    private int playerId;
    private int otherId;
    private string gameId;

    private NetworkController network;
    private GameStateController gameState;
    private PlayerSyncController playerSync;

    private float syncInterval = 0.02f;
    private float timer;

    void Start()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 60;

        LoadFromMatchmaking();

        otherId = (playerId == 0) ? 1 : 0;

        network = new NetworkController(apiClient, this);
        gameState = new GameStateController(playerId);
        playerSync = new PlayerSyncController(spawnManager, playerPrefabs);

        playerSync.SpawnLocal(playerId);

        SetStatus();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= syncInterval)
        {
            timer = 0f;
            Sync();
        }
    }

    void Sync()
    {
        SendMyData();
        RequestOtherData();
    }

    void SendMyData()
    {
        var local = playerSync.GetLocal();

        if (local == null) return;

        ServerData data = gameState.BuildLocalData(local.transform.position);

        network.Send(gameId, playerId, data);
    }

    void RequestOtherData()
    {
        network.Receive(gameId, otherId, OnOtherData);
    }

    void OnOtherData(ServerData data)
    {
        if (data == null) return;

        playerSync.UpdateRemote(otherId, data);

        gameState.ProcessRemoteData(data, otherId);
    }

    void LoadFromMatchmaking()
    {
        Matchmaking mm = FindObjectOfType<Matchmaking>();

        if (mm == null)
        {
            playerId = 0;
            gameId = "game1";
            return;
        }

        playerId = mm.GetPlayerId();
        gameId = mm.GetGameId();
    }

    void SetStatus()
    {
        if (statusText == null) return;

        if (playerId == 0)
        {
            statusText.text = "BLUE";
            statusText.color = Color.blue;
        }
        else
        {
            statusText.text = "RED";
            statusText.color = Color.red;
        }
    }

    public GameObject GetLocalPlayer() => playerSync.GetLocal();
    public int GetPlayerId() => playerId;

    public void SetEventZ(float z)
    {
        gameState.SetEventZ(z);
    }
}