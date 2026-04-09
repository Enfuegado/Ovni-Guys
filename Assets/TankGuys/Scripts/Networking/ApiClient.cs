using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : MonoBehaviour
{
    public string baseUrl = "http://localhost:5005/server";

    public IEnumerator GetPlayerData(string gameId, string playerId, System.Action<ServerData> onSuccess)
    {
        string url = $"{baseUrl}/{gameId}/{playerId}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("GET Error: " + webRequest.error + " | URL: " + url);
                onSuccess?.Invoke(null);
                yield break;
            }

            var data = JsonUtility.FromJson<ServerData>(webRequest.downloadHandler.text);
            onSuccess?.Invoke(data);
        }
    }

    public IEnumerator PostPlayerData(string gameId, string playerId, ServerData data)
    {
        string url = $"{baseUrl}/{gameId}/{playerId}";
        string jsonData = JsonUtility.ToJson(data);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("POST Error: " + webRequest.error + " | URL: " + url);
            }
        }
    }
}