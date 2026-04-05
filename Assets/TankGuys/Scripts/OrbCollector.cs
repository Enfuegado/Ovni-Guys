using UnityEngine;

public class OrbCollector : MonoBehaviour
{
    public int score = 0;
    public int winScore = 10;
    public float collectRadius = 0.6f;

    public Vector3 winPosition = new Vector3(9999, 9999, 0);

    private GameManagerHTTP gameManager;
    private GameObject localPlayer;
    private GameObject remotePlayer;

    private bool hasWon = false;

    void Start()
    {
        gameManager = FindObjectOfType<GameManagerHTTP>();
    }

    void Update()
    {
        if (gameManager == null) return;

        if (localPlayer == null)
            localPlayer = gameManager.GetLocalPlayer();

        if (remotePlayer == null)
            FindRemotePlayer();

        if (localPlayer == null) return;

        if (!hasWon)
            CheckLocalCollection();

        SimulateRemoteCollection();
    }

    void FindRemotePlayer()
    {
        PlayerTag[] players = FindObjectsOfType<PlayerTag>();

        foreach (var p in players)
        {
            if (p.PlayerId != gameManager.playerId)
            {
                remotePlayer = p.gameObject;
                break;
            }
        }
    }

    void CheckLocalCollection()
    {
        GameObject[] orbs = GameObject.FindGameObjectsWithTag("Orb");

        foreach (GameObject orb in orbs)
        {
            float dist = Vector2.Distance(localPlayer.transform.position, orb.transform.position);

            if (dist < collectRadius)
            {
                CollectOrb(orb);
                break;
            }
        }
    }

    void SimulateRemoteCollection()
    {
        if (remotePlayer == null) return;

        GameObject[] orbs = GameObject.FindGameObjectsWithTag("Orb");

        foreach (GameObject orb in orbs)
        {
            float dist = Vector2.Distance(remotePlayer.transform.position, orb.transform.position);

            if (dist < collectRadius)
            {
                Destroy(orb);
                break;
            }
        }
    }

    void CollectOrb(GameObject orb)
    {
        Destroy(orb);
        score++;

        if (score >= winScore)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        hasWon = true;

        localPlayer.transform.position = winPosition;

        Debug.Log("Ganaste");
    }
}