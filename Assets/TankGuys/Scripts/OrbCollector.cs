using UnityEngine;

public class OrbCollector : MonoBehaviour
{
    public int score = 0;
    public int winScore = 10;
    public float collectRadius = 0.6f;

    private GameManagerHTTP gameManager;
    private GameObject localPlayer;

    private bool hasWon = false;

    private ScoreUI scoreUI;

    void Start()
    {
        gameManager = FindObjectOfType<GameManagerHTTP>();
        scoreUI = FindObjectOfType<ScoreUI>();
    }

    void Update()
    {
        if (gameManager == null) return;

        if (localPlayer == null)
            localPlayer = gameManager.GetLocalPlayer();

        if (localPlayer == null) return;

        if (!hasWon)
            CheckLocalCollection();
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

    void CollectOrb(GameObject orb)
    {
        OrbId orbId = orb.GetComponent<OrbId>();
        if (orbId == null) return;

        Destroy(orb);
        score++;

        UpdateScoreUI();

        gameManager.SetEventZ(1000f + orbId.id);

        if (score >= winScore)
        {
            WinGame();
        }
    }

    void UpdateScoreUI()
    {
        if (scoreUI == null) return;

        if (gameManager.GetPlayerId() == 0)
            scoreUI.SetBlueScore(score);
        else
            scoreUI.SetRedScore(score);
    }

    void WinGame()
    {
        hasWon = true;

        gameManager.SetEventZ(9999f);

        var endUI = FindObjectOfType<GameEndUIController>();
        if (endUI != null)
            endUI.ShowResult(true, gameManager.GetPlayerId());
    }
}