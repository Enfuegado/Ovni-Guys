using UnityEngine;

public interface IGameStateService
{
    ServerData BuildLocalData(Vector3 pos);
    void ProcessRemoteData(ServerData data, int otherId);
    void SetEventZ(float z);
}