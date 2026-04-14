using UnityEngine;
using TMPro;

public class GameManagerHTTP : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI objectiveText;

    public float syncInterval = 0.02f;

    private int playerId;
    private int otherId;
    private string gameId = "game1";

    private INetworkService network;
    private IGameStateService gameState;
    private IPlayerSyncService playerSync;

    private float timer;
    private bool initialized = false;

    public void Init(
        INetworkService network,
        IGameStateService gameState,
        IPlayerSyncService playerSync,
        int playerId)
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 60;

        this.network = network;
        this.gameState = gameState;
        this.playerSync = playerSync;
        this.playerId = playerId;

        otherId = (playerId == 0) ? 1 : 0;

        playerSync.SpawnLocal(playerId);

        SetStatus();

        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

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
        network.Receive(gameId, otherId, OnReceive);
    }

    void OnReceive(ServerData data)
    {
        if (data == null) return;

        playerSync.UpdateRemote(otherId, data);
        gameState.ProcessRemoteData(data, otherId);
    }

    public void SendImmediateEvent(float z)
    {
        gameState.SetEventZ(z);

        var local = playerSync.GetLocal();
        if (local == null) return;

        var data = gameState.BuildLocalData(local.transform.position);

        network.Send(gameId, playerId, data);
    }

    void SetStatus()
    {
        if (statusText == null) return;

    if (playerId == 0)
    {
        statusText.text = "<color=#3090E0>BLUE TEAM</color>";
        statusText.color = Color.white;

        if (objectiveText != null)
        {
            objectiveText.text = "COLLECT 10 COWS BEFORE THE <color=#E03030>RED TEAM</color>";
            objectiveText.color = Color.white;
        }
    }
    else
    {
        statusText.text = "<color=#E03030>RED TEAM</color>";
        statusText.color = Color.white;

        if (objectiveText != null)
        {
            objectiveText.text = "COLLECT 10 COWS BEFORE THE <color=#3090E0>BLUE TEAM</color>";
            objectiveText.color = Color.white;
        }
    }
    }

    public GameObject GetLocalPlayer()
    {
        if (!initialized) return null;
        return playerSync.GetLocal();
    }

    public int GetPlayerId()
    {
        return playerId;
    }

    public void SetEventZ(float z)
    {
        gameState.SetEventZ(z);
    }
}