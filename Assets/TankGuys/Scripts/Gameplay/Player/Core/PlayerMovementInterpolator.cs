using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementInterpolator
{
    private Dictionary<int, Vector3> velocities = new();

    public Vector3 GetPosition(int id, Vector3 current, Vector3 target)
    {
        if (!velocities.ContainsKey(id))
        {
            velocities[id] = Vector3.zero;
            return target;
        }

        float smoothTime = 0.025f; // 🔥 más rápido = menos delay

        Vector3 velocity = velocities[id];

        Vector3 result = Vector3.SmoothDamp(
            current,
            target, // ❌ sin predicción
            ref velocity,
            smoothTime
        );

        velocities[id] = velocity;

        return result;
    }
}