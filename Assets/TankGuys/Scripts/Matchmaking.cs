using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using TMPro;

public class Matchmaking : MonoBehaviour
{
    public ApiClient apiClient;
    public TextMeshProUGUI statusText;

    public string gameId = "game1";

    private int playerId;
    private int otherId;

    private FileStream lockStream;
    private bool searching = false;

    private bool isReady = false;
    private bool hasDelay = false;
    private float startDelay = 0f;

    void Awake()
    {
        if (apiClient == null)
            apiClient = GetComponent<ApiClient>();

        DontDestroyOnLoad(gameObject);
    }

    public void FindMatch()
    {
        if (searching) return;

        searching = true;

        if (statusText != null)
            statusText.text = "Searching for match...";

        AssignPlayerId();

        StartCoroutine(SearchLoop());
    }

    void AssignPlayerId()
    {
        string lock0 = Path.Combine(Application.persistentDataPath, "lock0");
        string lock1 = Path.Combine(Application.persistentDataPath, "lock1");

        try
        {
            lockStream = new FileStream(lock0, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            playerId = 0;
        }
        catch
        {
            lockStream = new FileStream(lock1, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            playerId = 1;
        }

        otherId = (playerId == 0) ? 1 : 0;
    }

    IEnumerator SearchLoop()
    {
        while (true)
        {
            if (!isReady)
            {
                ServerData readyData = new ServerData
                {
                    posX = playerId,
                    posY = 1000,
                    posZ = 0f
                };

                yield return StartCoroutine(apiClient.PostPlayerData(gameId, playerId.ToString(), readyData));
                isReady = true;
            }

            ServerData other = null;

            yield return StartCoroutine(apiClient.GetPlayerData(gameId, otherId.ToString(), (data) =>
            {
                other = data;
            }));

            if (other != null && other.posY == 1000)
            {
                if (playerId == 0 && !hasDelay)
                {
                    startDelay = 3f;
                    hasDelay = true;

                    ServerData startData = new ServerData
                    {
                        posX = playerId,
                        posY = 1000,
                        posZ = startDelay
                    };

                    yield return StartCoroutine(apiClient.PostPlayerData(gameId, playerId.ToString(), startData));
                }

                if (other.posZ > 0f && !hasDelay)
                {
                    startDelay = other.posZ;
                    hasDelay = true;
                }

                if (hasDelay)
                {
                    if (statusText != null)
                        statusText.text = "Match found!";

                    yield return new WaitForSeconds(startDelay);
                    SceneManager.LoadScene("Game");
                    yield break;
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    public int GetPlayerId()
    {
        return playerId;
    }

    public string GetGameId()
    {
        return gameId;
    }
}