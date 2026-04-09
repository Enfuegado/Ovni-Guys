public interface INetworkService
{
    void Send(string gameId, int playerId, ServerData data);
    void Receive(string gameId, int otherId, System.Action<ServerData> callback);
}