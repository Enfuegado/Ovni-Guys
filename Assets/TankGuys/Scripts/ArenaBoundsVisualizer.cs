using UnityEngine;

public class AreaBoundsVisualizer : MonoBehaviour
{
    public float width = 24f;
    public float height = 12f;

    public Color borderColor = Color.green;
    public Color fillColor = new Color(0f, 1f, 0f, 0.1f);

    void OnDrawGizmos()
    {
        float halfW = width * 0.5f;
        float halfH = height * 0.5f;

        Vector3 center = transform.position;

        Vector3 topLeft = center + new Vector3(-halfW, halfH, 0);
        Vector3 topRight = center + new Vector3(halfW, halfH, 0);
        Vector3 bottomLeft = center + new Vector3(-halfW, -halfH, 0);
        Vector3 bottomRight = center + new Vector3(halfW, -halfH, 0);

        Gizmos.color = borderColor;

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        Gizmos.color = fillColor;
        Gizmos.DrawCube(center, new Vector3(width, height, 0.01f));
    }
}