using UnityEngine;

public class PlayerManager
{
    private SpawnManager spawnManager;
    private GameObject[] prefabs;

    private GameObject localPlayer;
    private GameObject remotePlayer;

    public PlayerManager(SpawnManager spawnManager, GameObject[] prefabs)
    {
        this.spawnManager = spawnManager;
        this.prefabs = prefabs;
    }

    public GameObject SpawnLocal(int playerId)
    {
        Vector2 pos = spawnManager.GetSpawnPosition(playerId);

        localPlayer = Object.Instantiate(prefabs[playerId], pos, Quaternion.identity);
        localPlayer.name = "LOCAL";

        var controller = localPlayer.AddComponent<PlayerLocalController>();
        controller.enabled = false;

        return localPlayer;
    }

    public GameObject SpawnRemote(int otherId, Vector3 pos)
    {
        remotePlayer = Object.Instantiate(prefabs[otherId], pos, Quaternion.identity);
        remotePlayer.name = "REMOTE";
        return remotePlayer;
    }

    public GameObject GetLocal() => localPlayer;
    public GameObject GetRemote() => remotePlayer;
}