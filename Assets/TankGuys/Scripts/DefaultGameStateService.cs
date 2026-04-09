using UnityEngine;

public class DefaultGameStateService : IGameStateService
{
    private EventProcessor events = new EventProcessor();

    private int playerId;
    private bool gameEnded = false;
    private float persistentZ = 0f;

    private int remoteScore = 0;
    private ScoreUI scoreUI;

    public DefaultGameStateService(int playerId)
    {
        this.playerId = playerId;
    }

    public void Initialize(ScoreUI scoreUI)
    {
        this.scoreUI = scoreUI;
    }

    public ServerData BuildLocalData(Vector3 pos)
    {
        return new ServerData
        {
            posX = pos.x,
            posY = pos.y,
            posZ = persistentZ
        };
    }

    public void ProcessRemoteData(ServerData data, int otherId)
    {
        float z = data.posZ;

        int? orbId = events.GetOrbId(z);

        if (orbId.HasValue)
        {
            RemoveOrb(orbId.Value);
            AddRemoteScore();
        }

        if (events.IsGameEnd(z))
        {
            if (!gameEnded)
            {
                gameEnded = true;

                var endUI = Object.FindFirstObjectByType<GameEndUIController>();
                if (endUI != null)
                    endUI.ShowResult(false, otherId);
            }
        }
    }

    void AddRemoteScore()
    {
        remoteScore++;

        if (scoreUI == null) return;

        if (playerId == 0)
            scoreUI.SetRedScore(remoteScore);
        else
            scoreUI.SetBlueScore(remoteScore);
    }

    void RemoveOrb(int id)
    {
        var orbs = GameObject.FindGameObjectsWithTag("Orb");

        foreach (var orb in orbs)
        {
            var orbId = orb.GetComponent<OrbId>();
            if (orbId != null && orbId.id == id)
            {
                Object.Destroy(orb);
                break;
            }
        }
    }

    public void SetEventZ(float z)
    {
        persistentZ = z;

        if (z >= 9000f)
            gameEnded = true;
    }
}