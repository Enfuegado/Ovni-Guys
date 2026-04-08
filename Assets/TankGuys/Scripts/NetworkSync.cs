using System;
using UnityEngine;
public class NetworkSync
{
    private ApiClient api;

    public NetworkSync(ApiClient api)
    {
        this.api = api;
    }

    public void Send(string gameId, int playerId, ServerData data, MonoBehaviour runner)
    {
        runner.StartCoroutine(api.PostPlayerData(gameId, playerId.ToString(), data));
    }

    public void Receive(string gameId, int otherId, Action<ServerData> callback, MonoBehaviour runner)
    {
        runner.StartCoroutine(api.GetPlayerData(gameId, otherId.ToString(), callback));
    }
}