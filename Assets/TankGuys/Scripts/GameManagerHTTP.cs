using UnityEngine;
using TMPro;

public class GameManagerHTTP : MonoBehaviour
{
    public ApiClient apiClient;
    public SpawnManager spawnManager;
    public GameObject[] playerPrefabs;
    public TextMeshProUGUI statusText;

    private int playerId;
    private int otherId;
    private string gameId;

    private NetworkSync network;
    private EventProcessor events;
    private PlayerManager players;

    private PlayerMovementInterpolator interpolator = new PlayerMovementInterpolator();

    private bool otherPlayerFound = false;
    private bool gameEnded = false;

    private float persistentZ = 0f;

    private ScoreUI scoreUI;
    private int remoteScore = 0;

    void Start()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 60;

        scoreUI = FindObjectOfType<ScoreUI>();

        network = new NetworkSync(apiClient);
        events = new EventProcessor();
        players = new PlayerManager(spawnManager, playerPrefabs);

        LoadFromMatchmaking();

        otherId = (playerId == 0) ? 1 : 0;

        players.SpawnLocal(playerId);

        InvokeRepeating(nameof(SyncLoop), 0f, 0.02f);

        SetStatus();
    }

    void SyncLoop()
    {
        SendMyData();
        GetOtherData();
    }

    void SendMyData()
    {
        var local = players.GetLocal();
        if (local == null) return;

        ServerData data;

        if (gameEnded)
        {
            // 🔴 SOLO enviar estado final
            data = new ServerData
            {
                posX = 0,
                posY = 0,
                posZ = 9999f
            };
        }
        else
        {
            Vector3 pos = local.transform.position;

            data = new ServerData
            {
                posX = pos.x,
                posY = pos.y,
                posZ = persistentZ
            };
        }

        network.Send(gameId, playerId, data, this);
    }

    void GetOtherData()
    {
        network.Receive(gameId, otherId, OnOtherData, this);
    }

    void OnOtherData(ServerData data)
    {
        if (data == null) return;

        Vector3 targetPos = new Vector3(data.posX, data.posY, 0f);

        if (!otherPlayerFound)
        {
            players.SpawnRemote(otherId, targetPos);
            otherPlayerFound = true;
        }

        var remote = players.GetRemote();

        if (remote != null)
        {
            Vector3 current = remote.transform.position;
            Vector3 newPos = interpolator.GetPosition(otherId, current, targetPos);
            remote.transform.position = newPos;
        }

        ProcessEvents(data.posZ);
    }

    void ProcessEvents(float z)
    {
        // 🔴 FIX: SIEMPRE procesar fin de juego
        if (events.IsGameEnd(z))
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

        // 🔴 ahora sí bloquear resto de eventos
        if (gameEnded) return;

        int? orbId = events.GetOrbId(z);

        if (orbId.HasValue)
        {
            RemoveOrbById(orbId.Value);
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

    public void SetEventZ(float z)
    {
        if (gameEnded) return;

        persistentZ = z;

        // 🔴 FORZAR envío inmediato del evento crítico
        if (z >= 9000f)
        {
            gameEnded = true;

            ServerData data = new ServerData
            {
                posX = 0,
                posY = 0,
                posZ = 9999f
            };

            network.Send(gameId, playerId, data, this);
        }
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

    public GameObject GetLocalPlayer() => players.GetLocal();
    public int GetPlayerId() => playerId;
    public string GetGameId() => gameId;
}