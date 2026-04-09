using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class Matchmaking : MonoBehaviour
{
    public ApiClient apiClient;
    public TextMeshProUGUI statusText;

    public string gameId = "game1";

    private int playerId;
    private int otherPlayerId;

    private bool isSearching = false;

    private PlayerIdAssigner playerIdAssigner;
    private SceneLoader sceneLoader;

    void Awake()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 60;

        if (apiClient == null)
            apiClient = GetComponent<ApiClient>();

        playerIdAssigner = new PlayerIdAssigner();
        sceneLoader = new SceneLoader();

        DontDestroyOnLoad(gameObject);
    }

    public void FindMatch()
    {
        if (isSearching) return;

        isSearching = true;

        SetStatus("Searching for match...");

        playerId = playerIdAssigner.AssignPlayerId();
        otherPlayerId = (playerId == 0) ? 1 : 0;

        StartCoroutine(MatchLoop());
    }

    IEnumerator MatchLoop()
    {
        float matchStartDelay = -1f;

        while (true)
        {
            yield return SendReadyState();

            ServerData otherPlayerData = null;

            yield return StartCoroutine(apiClient.GetPlayerData(gameId, otherPlayerId.ToString(), (data) =>
            {
                otherPlayerData = data;
            }));

            if (IsOtherPlayerReady(otherPlayerData))
            {
                if (playerId == 0 && matchStartDelay < 0f)
                {
                    matchStartDelay = 3f;
                    yield return SendStartSignal(matchStartDelay);
                }

                if (otherPlayerData.posZ > 0f && matchStartDelay < 0f)
                {
                    matchStartDelay = otherPlayerData.posZ;
                }

                if (matchStartDelay >= 0f)
                {
                    SetStatus("Match found!");

                    yield return new WaitForSeconds(matchStartDelay);

                    sceneLoader.LoadGame();
                    yield break;
                }
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator SendReadyState()
    {
        ServerData requestData = new ServerData
        {
            posX = playerId,
            posY = 1000,
            posZ = 0f
        };

        return apiClient.PostPlayerData(gameId, playerId.ToString(), requestData);
    }

    IEnumerator SendStartSignal(float delay)
    {
        ServerData requestData = new ServerData
        {
            posX = playerId,
            posY = 1000,
            posZ = delay
        };

        return apiClient.PostPlayerData(gameId, playerId.ToString(), requestData);
    }

    bool IsOtherPlayerReady(ServerData otherPlayerData)
    {
        return otherPlayerData != null &&
               otherPlayerData.posY == 1000 &&
               otherPlayerData.posX != playerId;
    }

    void SetStatus(string text)
    {
        if (statusText != null)
            statusText.text = text;
    }

    public void Cleanup()
    {
        playerIdAssigner.Cleanup();
    }

    public int GetPlayerId() => playerId;
    public string GetGameId() => gameId;
}