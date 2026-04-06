using UnityEngine;
using TMPro;

public class GameManagerHTTP : MonoBehaviour
{
    public ApiClient apiClient;
    public SpawnManager spawnManager;
    public GameObject[] playerPrefabs;
    public TextMeshProUGUI statusText;

    public Vector3 winPosition = new Vector3(9999, 9999, 0);

    private int playerId;
    private int otherId;
    private string gameId;

    private GameObject localPlayer;
    private GameObject remotePlayer;

    private PlayerMovementInterpolator interpolator = new PlayerMovementInterpolator();

    private bool otherPlayerFound = false;
    private bool gameEnded = false;

    void Start()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 60;

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

        localPlayer.AddComponent<PlayerLocalController>();

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
            posZ = pos.z
        };

        StartCoroutine(apiClient.PostPlayerData(gameId, playerId.ToString(), data));
    }

    void GetOtherPlayer()
    {
        StartCoroutine(apiClient.GetPlayerData(gameId, otherId.ToString(), OnOtherPlayerData));
    }

    void OnOtherPlayerData(ServerData data)
    {
        if (data == null) return;

        Vector3 targetPos = new Vector3(data.posX, data.posY, data.posZ);

        if (!otherPlayerFound)
        {
            SpawnRemotePlayer(targetPos);
            otherPlayerFound = true;
        }

        if (remotePlayer != null)
        {
            Vector3 current = remotePlayer.transform.position;

            Vector3 last = interpolator.GetLastPosition(otherId);
            Vector3 newPos = interpolator.GetPosition(otherId, current, targetPos);

            remotePlayer.transform.position = newPos;

            CheckOrbPath(last, targetPos);
        }

        CheckRemoteWin(targetPos);
    }

    void CheckOrbPath(Vector3 from, Vector3 to)
    {
        GameObject[] orbs = GameObject.FindGameObjectsWithTag("Orb");

        foreach (GameObject orb in orbs)
        {
            float dist = Vector3.Distance(orb.transform.position, from);

            if (dist < 0.6f)
            {
                Destroy(orb);
                break;
            }
        }
    }

    void CheckRemoteWin(Vector3 pos)
    {
        if (gameEnded) return;

        float dist = Vector2.Distance(pos, winPosition);

        if (dist < 2f)
        {
            gameEnded = true;

            var endUI = FindObjectOfType<GameEndUIController>();
            if (endUI != null)
                endUI.ShowResult(false, otherId);
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