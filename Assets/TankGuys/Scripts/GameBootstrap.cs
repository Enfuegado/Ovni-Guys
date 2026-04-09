using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    public ApiClient apiClient;
    public SpawnManager spawnManager;
    public GameObject[] playerPrefabs;

    void Awake()
    {
        var gameManager = FindFirstObjectByType<GameManagerHTTP>();
        var matchmaking = FindFirstObjectByType<Matchmaking>();

        int playerId = 0;

        if (matchmaking != null)
            playerId = matchmaking.GetPlayerId();

        INetworkService network = new HttpNetworkService(apiClient, this);

        var gameState = new DefaultGameStateService(playerId);

        var scoreUI = FindFirstObjectByType<ScoreUI>();
        gameState.Initialize(scoreUI);

        IPlayerSyncService playerSync = new DefaultPlayerSyncService(spawnManager, playerPrefabs);

        gameManager.Init(network, gameState, playerSync, playerId);
    }
}