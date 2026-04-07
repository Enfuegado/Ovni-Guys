using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManagerHTTP : MonoBehaviour
{
    public ApiClient apiClient;
    public SpawnManager spawnManager;
    public GameObject[] playerPrefabs;
    public TextMeshProUGUI statusText;

    private int playerId;
    private int otherId;
    private string gameId;

    private GameObject localPlayer;
    private GameObject remotePlayer;

    private PlayerMovementInterpolator interpolator = new PlayerMovementInterpolator();

    private bool otherPlayerFound = false;
    private bool gameEnded = false;

    private float persistentZ = 0f;

    private HashSet<int> processedOrbs = new HashSet<int>();

    private int remoteScore = 0;
    private ScoreUI scoreUI;

    void Start()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 60;

        scoreUI = FindObjectOfType<ScoreUI>();

        LoadFromMatchmaking();

        otherId = (playerId == 0) ? 1 : 0;

        SpawnLocalPlayer();

        InvokeRepeating(nameof(SyncLoop), 0f, 0.02f);

        SetStatus();
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

    void SpawnLocalPlayer()
    {
        Vector2 pos = spawnManager.GetSpawnPosition(playerId);

        GameObject prefab = playerPrefabs[playerId];

        localPlayer = Instantiate(prefab, pos, Quaternion.identity);
        localPlayer.name = "LOCAL";

        var controller = localPlayer.AddComponent<PlayerLocalController>();
        controller.enabled = false;

        var tag = localPlayer.GetComponent<PlayerTag>();
        if (tag != null)
            tag.PlayerId = playerId;
    }

    void SpawnRemotePlayer(Vector3 pos)
    {
        GameObject prefab = playerPrefabs[otherId];

        remotePlayer = Instantiate(prefab, pos, Quaternion.identity);
        remotePlayer.name = "REMOTE";

        remotePlayer.AddComponent<PlayerRemoteController>();

        var tag = remotePlayer.GetComponent<PlayerTag>();
        if (tag != null)
            tag.PlayerId = otherId;
    }

    void SyncLoop()
    {
        SendMyPosition();
        GetOtherPlayer();
    }

    void SendMyPosition()
    {
        if (localPlayer == null) return;

        Vector3 pos = localPlayer.transform.position;

        ServerData data = new ServerData
        {
            posX = pos.x,
            posY = pos.y,
            posZ = persistentZ
        };

        StartCoroutine(apiClient.PostPlayerData(gameId, playerId.ToString(), data));
    }

    public void SetEventZ(float z)
    {
        persistentZ = z;

        if (z >= 9000f)
        {
            gameEnded = true;
        }
    }

    void GetOtherPlayer()
    {
        StartCoroutine(apiClient.GetPlayerData(gameId, otherId.ToString(), OnOtherPlayerData));
    }

    void OnOtherPlayerData(ServerData data)
    {
        if (data == null) return;

        Vector3 targetPos = new Vector3(data.posX, data.posY, 0f);

        if (!otherPlayerFound)
        {
            SpawnRemotePlayer(targetPos);
            otherPlayerFound = true;
        }

        if (remotePlayer != null)
        {
            Vector3 current = remotePlayer.transform.position;
            Vector3 newPos = interpolator.GetPosition(otherId, current, targetPos);
            remotePlayer.transform.position = newPos;
        }

        ProcessRemoteEvents(data.posZ);
    }

    void ProcessRemoteEvents(float z)
    {
        if (z >= 9000f)
        {
            if (!gameEnded)
            {
                gameEnded = true;

                var endUI = FindObjectOfType<GameEndUIController>();
                if (endUI != null)
                    endUI.ShowResult(false, otherId);
            }
            return;
        }

        if (gameEnded) return;

        if (z >= 1000f)
        {
            int orbId = (int)(z - 1000f);

            if (processedOrbs.Contains(orbId)) return;

            processedOrbs.Add(orbId);

            RemoveOrbById(orbId);
            AddRemoteScore();
        }
    }

    void AddRemoteScore()
    {
        remoteScore++;

        if (scoreUI == null) return;

        if (playerId == 0)
            scoreUI.SetRedScore(remoteScore);
        else
            scoreUI.SetBlueScore(remoteScore);
    }

    void RemoveOrbById(int id)
    {
        GameObject[] orbs = GameObject.FindGameObjectsWithTag("Orb");

        foreach (GameObject orb in orbs)
        {
            OrbId orbId = orb.GetComponent<OrbId>();
            if (orbId != null && orbId.id == id)
            {
                Destroy(orb);
                break;
            }
        }
    }

    public GameObject GetLocalPlayer()
    {
        return localPlayer;
    }

    public int GetPlayerId()
    {
        return playerId;
    }

    public string GetGameId()
    {
        return gameId;
    }
}