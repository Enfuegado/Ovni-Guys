using UnityEngine;

public class PlayerLocalController : MonoBehaviour
{
    private Transform turret;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float turretRotationSpeed = 15f;

    void Start()
    {
        turret = transform.Find("Turret");
    }

    void Update()
    {
        HandleMovement();
        HandleTurret();
    }

    private void HandleMovement()
    {
        Vector2 input = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        Vector3 move = new Vector3(input.x, input.y, 0f);
        transform.position += move * moveSpeed * Time.deltaTime;

        if (move.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(0, 0, angle),
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void HandleTurret()
    {
        if (turret == null) return;

        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(cam.transform.position.z);

        Vector3 mouseWorld = cam.ScreenToWorldPoint(mousePos);
        Vector2 dir = mouseWorld - turret.position;

        if (dir.sqrMagnitude < 0.001f) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        turret.rotation = Quaternion.Lerp(
            turret.rotation,
            Quaternion.Euler(0, 0, angle),
            turretRotationSpeed * Time.deltaTime
        );
    }
}