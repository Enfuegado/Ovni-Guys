using UnityEngine;
using TMPro;
using System.IO;

public class GameManagerHTTP : MonoBehaviour
{
    public string gameId = "game1";

    public ApiClient apiClient;
    public SpawnManager spawnManager;
    public GameObject[] playerPrefabs;
    public TextMeshProUGUI statusText;

    public Vector3 winPosition = new Vector3(9999, 9999, 0);

    private int playerId;
    private int otherId;

    private GameObject localPlayer;
    private GameObject remotePlayer;

    private PlayerMovementInterpolator interpolator = new PlayerMovementInterpolator();

    private bool otherPlayerFound = false;
    private bool gameEnded = false;

    void Start()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 60;

        LoadPlayerId();

        otherId = (playerId == 0) ? 1 : 0;

        SpawnLocalPlayer();

        InvokeRepeating(nameof(SyncLoop), 0f, 0.02f);

        SetStatus();
    }

    void LoadPlayerId()
    {
        string lock0 = Path.Combine(Application.persistentDataPath, "lock0");
        string lock1 = Path.Combine(Application.persistentDataPath, "lock1");

        try
        {
            FileStream fs0 = new FileStream(lock0, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            playerId = 0;

            GameObject obj = new GameObject("Lock0");
            DontDestroyOnLoad(obj);
            obj.AddComponent<FileLock>().Init(fs0);
        }
        catch
        {
            try
            {
                FileStream fs1 = new FileStream(lock1, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                playerId = 1;

                GameObject obj = new GameObject("Lock1");
                DontDestroyOnLoad(obj);
                obj.AddComponent<FileLock>().Init(fs1);
            }
            catch
            {
                playerId = Random.Range(0, 2);
            }
        }

        Debug.Log("PlayerID asignado: " + playerId);
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
            Vector3 orbPos = orb.transform.position;

            float dist = DistancePointToSegment(orbPos, from, to);

            if (dist < 0.6f)
            {
                Destroy(orb);
                break;
            }
        }
    }

    float DistancePointToSegment(Vector3 point, Vector3 a, Vector3 b)
    {
        Vector3 ab = b - a;

        float denom = Vector3.Dot(ab, ab);
        if (denom == 0f) return Vector3.Distance(point, a);

        float t = Vector3.Dot(point - a, ab) / denom;
        t = Mathf.Clamp01(t);

        Vector3 closest = a + t * ab;

        return Vector3.Distance(point, closest);
    }

    void CheckRemoteWin(Vector3 pos)
    {
        if (gameEnded) return;

        float dist = Vector2.Distance(pos, winPosition);

        if (dist < 1f)
        {
            gameEnded = true;

            var endUI = FindObjectOfType<GameEndUIController>();
            if (endUI != null)
                endUI.ShowResult(false, otherId);
        }
    }

    void SetStatus(string msg) { }

    public GameObject GetLocalPlayer()
    {
        return localPlayer;
    }

    public int GetPlayerId()
    {
        return playerId;
    }
}