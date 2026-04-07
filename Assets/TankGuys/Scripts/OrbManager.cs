using UnityEngine;
using System.Collections.Generic;

public class OrbManager : MonoBehaviour
{
    public GameObject orbPrefab;
    public int orbCount = 20;

    public float width = 24f;
    public float height = 12f;

    private List<GameObject> orbs = new List<GameObject>();

    void Start()
    {
        GenerateOrbs();
    }

    void GenerateOrbs()
    {
        Random.InitState(12345);

        float halfW = width * 0.5f;
        float halfH = height * 0.5f;

        for (int i = 0; i < orbCount; i++)
        {
            float x = Random.Range(-halfW, halfW);
            float y = Random.Range(-halfH, halfH);

            Vector2 pos = new Vector2(x, y);

            GameObject orb = Instantiate(orbPrefab, pos, Quaternion.identity);
            orbs.Add(orb);
        }
    }
}