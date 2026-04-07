using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrbManager : MonoBehaviour
{
    public GameObject orbPrefab;

    public int totalOrbs = 30;
    public float spawnDuration = 20f;

    public float width = 24f;
    public float height = 12f;

    public float minDistance = 1.5f;
    public int maxAttemptsPerOrb = 50;

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
        Random.InitState(12345);

        float halfW = width * 0.5f;
        float halfH = height * 0.5f;

        positions.Clear();

        for (int i = 0; i < totalOrbs; i++)
        {
            bool placed = false;

            for (int attempt = 0; attempt < maxAttemptsPerOrb; attempt++)
            {
                float x = Random.Range(-halfW, halfW);
                float y = Random.Range(-halfH, halfH);

                Vector2 candidate = new Vector2(x, y);

                if (IsFarEnough(candidate))
                {
                    positions.Add(candidate);
                    placed = true;
                    break;
                }
            }

            if (!placed)
            {
                positions.Add(Vector2.zero);
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