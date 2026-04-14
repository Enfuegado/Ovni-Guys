using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrbManager : MonoBehaviour
{
    public GameObject orbPrefab;

    public int totalOrbs = 30;
    public float spawnDuration = 20f;

    public float width = 20f;
    public float height = 10f;

    public float minDistance = 1.2f;
    public int maxAttemptsPerOrb = 100;

    public float margin = 1.5f;
    public float playerSafeRadius = 2.5f;

    private Vector2 player1Pos = new Vector2(-3f, 0f);
    private Vector2 player2Pos = new Vector2(3f, 0f);

    private List<Vector2> positions = new List<Vector2>();
    private List<GameObject> orbs = new List<GameObject>();

    void Start()
    {
        GeneratePositions();
        PreInstantiateOrbs();
        StartCoroutine(SpawnRoutine());
    }

    void GeneratePositions()
    {
        Random.InitState(54321); // 🔥 cambia este número si no te gusta el resultado

        float halfW = width * 0.5f;
        float halfH = height * 0.5f;

        Vector2 center = transform.position;

        positions.Clear();

        for (int i = 0; i < totalOrbs; i++)
        {
            bool placed = false;

            for (int attempt = 0; attempt < maxAttemptsPerOrb; attempt++)
            {
                float x = Random.Range(center.x - halfW + margin, center.x + halfW - margin);
                float y = Random.Range(center.y - halfH + margin, center.y + halfH - margin);

                Vector2 candidate = new Vector2(x, y);

                if (IsFarEnough(candidate) && IsFarFromPlayers(candidate))
                {
                    positions.Add(candidate);
                    placed = true;
                    break;
                }
            }

            if (!placed)
            {
                Vector2 fallback = new Vector2(
                    Random.Range(center.x - halfW + margin, center.x + halfW - margin),
                    Random.Range(center.y - halfH + margin, center.y + halfH - margin)
                );

                positions.Add(fallback);
            }
        }
    }

    bool IsFarEnough(Vector2 pos)
    {
        foreach (var p in positions)
        {
            if (Vector2.Distance(p, pos) < minDistance)
                return false;
        }
        return true;
    }

    bool IsFarFromPlayers(Vector2 pos)
    {
        if (Vector2.Distance(pos, player1Pos) < playerSafeRadius) return false;
        if (Vector2.Distance(pos, player2Pos) < playerSafeRadius) return false;
        return true;
    }

    void PreInstantiateOrbs()
    {
        for (int i = 0; i < positions.Count; i++)
        {
            GameObject orb = Instantiate(orbPrefab, positions[i], Quaternion.identity);

            OrbId id = orb.AddComponent<OrbId>();
            id.id = i;

            orb.SetActive(false);

            orbs.Add(orb);
        }
    }

    IEnumerator SpawnRoutine()
    {
        float delay = spawnDuration / totalOrbs;

        for (int i = 0; i < orbs.Count; i++)
        {
            orbs[i].SetActive(true);
            yield return new WaitForSeconds(delay);
        }
    }
}