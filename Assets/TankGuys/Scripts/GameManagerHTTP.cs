using UnityEngine;
using TMPro;

public class GameManagerHTTP : MonoBehaviour
{
    [Header("Config")]
    public string gameId = "game1";
    public int playerId = 0;
    private int otherId;

    [Header("References")]
    public ApiClient apiClient;
    public SpawnManager spawnManager;
    public GameObject playerPrefab;
    public TextMeshProUGUI statusText;

    private GameObject localPlayer;
    private GameObject remotePlayer;

    private PlayerMovementInterpolator interpolator = new PlayerMovementInterpolator();

    private bool otherPlayerFound = false;
    private bool gameEnded = false;

    public Vector3 winPosition = new Vector3(9999, 9999, 0);

    void Start()
    {
        otherId = (playerId == 0) ? 1 : 0;

        SpawnLocalPlayer();

        InvokeRepeating(nameof(SyncLoop), 0f, 0.1f);

        SetStatus("Esperando jugador...");
    }

    void SpawnLocalPlayer()
    {
        Vector2 pos = spawnManager.GetSpawnPosition(playerId);

        localPlayer = Instantiate(playerPrefab, pos, Quaternion.identity);
        localPlayer.name = "LOCAL";

        localPlayer.AddComponent<PlayerLocalController>();

        var tag = localPlayer.GetComponent<PlayerTag>();
        if (tag != null)
            tag.PlayerId = playerId;
    }

    void SpawnRemotePlayer(Vector3 pos)
    {
        remotePlayer = Instantiate(playerPrefab, pos, Quaternion.identity);
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
        Vector3 targetPos = new Vector3(data.posX, data.posY, data.posZ);

        if (!otherPlayerFound)
        {
            SpawnRemotePlayer(targetPos);
            otherPlayerFound = true;

            SetStatus("Jugador conectado");
            return;
        }

        if (remotePlayer == null) return;

        Vector3 current = remotePlayer.transform.position;
        Vector3 newPos = interpolator.GetPosition(otherId, current, targetPos);

        remotePlayer.transform.position = newPos;

        CheckRemoteWin(targetPos);
    }

    void CheckRemoteWin(Vector3 pos)
    {
        if (gameEnded) return;

        float dist = Vector2.Distance(pos, winPosition);

        if (dist < 1f)
        {
            gameEnded = true;
            SetStatus("Perdiste");
        }
    }

    void SetStatus(string msg)
    {
        if (statusText != null)
            statusText.text = msg;
    }

    public GameObject GetLocalPlayer()
    {
        return localPlayer;
    }
}