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

        Debug.Log("Player asignado: " + playerId);
    }

    IEnumerator SearchLoop()
    {
        float ignoreTime = 2f;
        bool detected = false;

        while (true)
        {
            ServerData dummy = new ServerData
            {
                posX = playerId,
                posY = 999,
                posZ = 0
            };

            yield return StartCoroutine(apiClient.PostPlayerData(gameId, playerId.ToString(), dummy));

            ignoreTime -= 0.5f;

            yield return StartCoroutine(apiClient.GetPlayerData(gameId, otherId.ToString(), (data) =>
            {
                if (data != null && ignoreTime <= 0f)
                {
                    if (data.posY == 999)
                        detected = true;
                }
            }));

            if (detected)
            {
                Debug.Log("Jugador detectado → esperando sincronización");

                if (statusText != null)
                    statusText.text = "Match found!";

                yield return new WaitForSeconds(2f);

                SceneManager.LoadScene("Game");
                yield break;
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