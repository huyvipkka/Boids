using UnityEngine;

public class BoidScript : MonoBehaviour
{
    public Vector2 Position => transform.position;
    public Vector2 Velocity { get; private set; }

    [SerializeField] BoidSettings settings;

    void Start()
    {
        Velocity = transform.up * settings.minSpeed;
    }

    public void ApplyForce(Vector2 force)
    {
        force = Vector2.ClampMagnitude(force, settings.maxSteerForce);
        Velocity += force * Time.fixedDeltaTime;
        LimitSpeed();
        Move();
    }

    void LimitSpeed()
    {
        float speed = Velocity.magnitude;
        if (speed < settings.minSpeed)
            Velocity = Velocity.normalized * settings.minSpeed;
        if (speed > settings.maxSpeed)
            Velocity = Velocity.normalized * settings.maxSpeed;
    }

    void Move()
    {
        transform.position += (Vector3)(Velocity * Time.fixedDeltaTime);

        if (Velocity.sqrMagnitude != 0)
        {
            float angle = Mathf.Atan2(Velocity.y, Velocity.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.Euler(0f, 0f, angle),
                1f * Time.fixedDeltaTime
            );
        }
    }
    void OnDrawGizmosSelected()
    {
        if (settings == null) return;

        // Vẽ vòng separation (đỏ)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, settings.separationRange);

        // Vẽ vòng alignment (xanh lá)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, settings.alignmentRange);

        // Vẽ vòng cohesion (xanh dương)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, settings.cohesionRange);
    }
}
