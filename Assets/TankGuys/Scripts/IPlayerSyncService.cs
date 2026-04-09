using UnityEngine;

public interface IPlayerSyncService
{
    void SpawnLocal(int id);
    void UpdateRemote(int id, ServerData data);
    GameObject GetLocal();
}