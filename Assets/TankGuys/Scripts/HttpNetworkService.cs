using System;
using UnityEngine;

public class HttpNetworkService : INetworkService
{
    private ApiClient api;
    private MonoBehaviour runner;

    public HttpNetworkService(ApiClient api, MonoBehaviour runner)
    {
        this.api = api;
        this.runner = runner;
    }

    public void Send(string gameId, int playerId, ServerData data)
    {
        runner.StartCoroutine(api.PostPlayerData(gameId, playerId.ToString(), data));
    }

    public void Receive(string gameId, int otherId, Action<ServerData> callback)
    {
        runner.StartCoroutine(api.GetPlayerData(gameId, otherId.ToString(), callback));
    }
}