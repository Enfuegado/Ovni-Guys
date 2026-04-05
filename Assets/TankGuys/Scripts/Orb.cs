using UnityEngine;

public class Orb : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0, 0, 60f * Time.deltaTime);
    }
}