using UnityEngine;

public class PlayerSyncController
{
    private PlayerManager players;
    private PlayerMovementInterpolator interpolator = new PlayerMovementInterpolator();

    private bool remoteSpawned = false;

    public PlayerSyncController(SpawnManager spawnManager, GameObject[] prefabs)
    {
        players = new PlayerManager(spawnManager, prefabs);
    }

    public void SpawnLocal(int id)
    {
        players.SpawnLocal(id);
    }

    public void UpdateRemote(int id, ServerData data)
    {
        Vector3 target = new Vector3(data.posX, data.posY, 0f);

        if (!remoteSpawned)
        {
            players.SpawnRemote(id, target);
            remoteSpawned = true;
        }

        var remote = players.GetRemote();

        if (remote == null) return;

        Vector3 current = remote.transform.position;
        Vector3 newPos = interpolator.GetPosition(id, current, target);

        remote.transform.position = newPos;
    }

    public GameObject GetLocal() => players.GetLocal();
}