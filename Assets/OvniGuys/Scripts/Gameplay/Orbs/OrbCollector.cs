using UnityEngine;

public class OrbCollector : MonoBehaviour
{
    public int score = 0;
    public int winScore = 10;

    private GameManagerHTTP gameManager;
    private bool hasWon = false;
    private ScoreUI scoreUI;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManagerHTTP>();
        scoreUI = FindFirstObjectByType<ScoreUI>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasWon) return;

        if (other.CompareTag("Orb"))
        {
            CollectOrb(other.gameObject);
        }
    }

    void CollectOrb(GameObject orb)
    {
        OrbId orbId = orb.GetComponent<OrbId>();
        if (orbId == null) return;

        Destroy(orb);
        score++;

        UpdateScoreUI();

        if (score >= winScore)
        {
            hasWon = true;

            gameManager.SendImmediateEvent(9000f + orbId.id);

            var endUI = FindFirstObjectByType<GameEndUIController>();
            if (endUI != null)
                endUI.ShowResult(true, gameManager.GetPlayerId());
        }
        else
        {
            gameManager.SetEventZ(1000f + orbId.id);
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
}