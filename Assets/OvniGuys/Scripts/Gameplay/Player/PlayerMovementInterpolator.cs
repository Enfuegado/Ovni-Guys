using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementInterpolator
{
    private Dictionary<int, Vector3> velocities = new();
    private Dictionary<int, bool> initialized = new();

    public Vector3 GetPosition(int id, Vector3 current, Vector3 target)
    {
        if (!initialized.ContainsKey(id))
        {
            initialized[id] = true;
            velocities[id] = Vector3.zero;
            return target;
        }

        float smoothTime = 0.025f;

        Vector3 velocity = velocities[id];

        Vector3 result = Vector3.SmoothDamp(
            current,
            target,
            ref velocity,
            smoothTime
        );

        velocities[id] = velocity;

        return result;
    }
}