using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    public ApiClient apiClient;
    public SpawnManager spawnManager;
    public GameObject[] playerPrefabs;

    void Awake()
    {
        var gameManager = FindFirstObjectByType<GameManagerHTTP>();
        var mm = FindFirstObjectByType<Matchmaking>();

        int playerId = 0;

        if (mm != null)
            playerId = mm.GetPlayerId();

        INetworkService network = new HttpNetworkService(apiClient, this);
        IGameStateService gameState = new DefaultGameStateService(playerId);
        IPlayerSyncService playerSync = new DefaultPlayerSyncService(spawnManager, playerPrefabs);

        gameManager.Init(network, gameState, playerSync, playerId);
    }
}