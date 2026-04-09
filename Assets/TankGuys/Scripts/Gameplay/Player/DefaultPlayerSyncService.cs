using UnityEngine;

public class DefaultPlayerSyncService : IPlayerSyncService
{
    private PlayerManager players;
    private PlayerMovementInterpolator interpolator = new PlayerMovementInterpolator();

    private bool spawned = false;

    public GameObject youLabelPrefab;

    public DefaultPlayerSyncService(SpawnManager spawnManager, GameObject[] prefabs)
    {
        players = new PlayerManager(spawnManager, prefabs);
    }

    public void SpawnLocal(int id)
    {
        players.SpawnLocal(id);

        var local = players.GetLocal();
        if (local == null) return;

        if (youLabelPrefab != null)
        {
            var label = GameObject.Instantiate(youLabelPrefab);

            var playerLabel = label.GetComponent<PlayerLabel>();
            if (playerLabel != null)
            {
                playerLabel.target = local.transform;
            }
        }
    }

    public void UpdateRemote(int id, ServerData data)
    {
        if (data.posY > 50f) return;

        Vector3 target = new Vector3(data.posX, data.posY, 0f);

        if (!spawned)
        {
            players.SpawnRemote(id, target);
            spawned = true;
            return;
        }

        var remote = players.GetRemote();
        if (remote == null) return;

        Vector3 current = remote.transform.position;
        Vector3 newPos = interpolator.GetPosition(id, current, target);

        remote.transform.position = newPos;
    }

    public GameObject GetLocal() => players.GetLocal();
}