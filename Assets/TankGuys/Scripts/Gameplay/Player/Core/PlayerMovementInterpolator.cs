using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementInterpolator
{
    private Dictionary<int, Vector3> lastPositions = new();

    public Vector3 GetPosition(int id, Vector3 current, Vector3 target)
    {
        if (!lastPositions.ContainsKey(id))
        {
            lastPositions[id] = target;
            return target;
        }

        Vector3 last = lastPositions[id];
        lastPositions[id] = target;

        float speed = 60f;
        return Vector3.Lerp(current, target, speed * Time.deltaTime);
    }

    public Vector3 GetLastPosition(int id)
    {
        if (lastPositions.ContainsKey(id))
            return lastPositions[id];

        return Vector3.zero;
    }
}