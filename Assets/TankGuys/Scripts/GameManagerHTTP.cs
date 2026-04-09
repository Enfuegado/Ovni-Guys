using UnityEngine;
using TMPro;
using System.Collections;

public class GameManagerHTTP : MonoBehaviour
{
    public ApiClient apiClient;
    public SpawnManager spawnManager;
    public GameObject[] playerPrefabs;
    public TextMeshProUGUI statusText;
    public float syncInterval = 0.02f;

    private int playerId;
    private int otherId;
    private string gameId;

    private INetworkService network;
    private IGameStateService gameState;
    private IPlayerSyncService playerSync;

    private float timer;
    private bool isRequesting;

    void Awake()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 60;

        LoadFromMatchmaking();

        otherId = (playerId == 0) ? 1 : 0;

        network = new HttpNetworkService(apiClient, this);
        gameState = new DefaultGameStateService(playerId);
        playerSync = new DefaultPlayerSyncService(spawnManager, playerPrefabs);

        playerSync.SpawnLocal(playerId);

        SetStatus();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= syncInterval)
        {
            timer = 0f;
            Tick();
        }
    }

    void Tick()
    {
        var local = playerSync.GetLocal();

        if (local == null) return;

        var data = gameState.BuildLocalData(local.transform.position);

        network.Send(gameId, playerId, data);

        if (!isRequesting)
        {
            isRequesting = true;
            network.Receive(gameId, otherId, OnReceive);
        }
    }

    void OnReceive(ServerData data)
    {
        isRequesting = false;

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

    public GameObject GetLocalPlayer()
    {
        return playerSync.GetLocal();
    }

    public int GetPlayerId()
    {
        return playerId;
    }

    public void SetEventZ(float z)
    {
        if (z >= 9000f)
        {
            StartCoroutine(SendGameEndRepeated());
            return;
        }

        gameState.SetEventZ(z);
    }

    IEnumerator SendGameEndRepeated()
    {
        for (int i = 0; i < 5; i++)
        {
            gameState.SetEventZ(9999f);
            yield return new WaitForSeconds(0.05f);
        }
    }
}