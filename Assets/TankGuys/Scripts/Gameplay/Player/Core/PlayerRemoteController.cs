using UnityEngine;

public class PlayerRemoteController : MonoBehaviour
{
    private Transform turret;

    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float turretRotationSpeed = 15f;

    void Start()
    {
        turret = transform.Find("Turret");
    }

    public void UpdateRotation(float tankAngle, float turretAngle)
    {
        SmoothTankRotation(tankAngle);
        SmoothTurretRotation(turretAngle);
    }

    private void SmoothTankRotation(float angle)
    {
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(0, 0, angle),
            rotationSpeed * Time.deltaTime
        );
    }

    private void SmoothTurretRotation(float angle)
    {
        if (turret == null) return;

        Quaternion target = Quaternion.Euler(0, 0, angle);

        turret.rotation = Quaternion.Lerp(
            turret.rotation,
            target,
            turretRotationSpeed * Time.deltaTime
        );
    }
}