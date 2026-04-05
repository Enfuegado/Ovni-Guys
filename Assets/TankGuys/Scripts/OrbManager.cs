using UnityEngine;
using System.Collections.Generic;

public class OrbManager : MonoBehaviour
{
    public GameObject orbPrefab;
    public int orbCount = 20;
    public float radius = 6f;

    private List<GameObject> orbs = new List<GameObject>();

    void Start()
    {
        GenerateOrbs();
    }

    void GenerateOrbs()
    {
        Random.InitState(12345);

        for (int i = 0; i < orbCount; i++)
        {
            Vector2 pos = Random.insideUnitCircle * radius;

            GameObject orb = Instantiate(orbPrefab, pos, Quaternion.identity);
            orbs.Add(orb);
        }
    }
}