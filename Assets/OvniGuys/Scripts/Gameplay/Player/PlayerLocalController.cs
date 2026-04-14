using UnityEngine;

public class PlayerLocalController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    public float width = 22f;
    public float height = 10f;

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 input = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        Vector3 move = new Vector3(input.x, input.y, 0f);
        transform.position += move * moveSpeed * Time.deltaTime;

        ClampPosition();
    }

    private void ClampPosition()
    {
        float halfW = width * 0.5f;
        float halfH = height * 0.5f;

        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, -halfW, halfW);
        pos.y = Mathf.Clamp(pos.y, -halfH, halfH);

        transform.position = pos;
    }
}