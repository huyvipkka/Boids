using UnityEngine;

public class BoidScript : MonoBehaviour
{
    public Vector2 Position => transform.position;
    public Vector2 Velocity = Vector2.left;
    [SerializeField] BoidSettings settings;

    void FixedUpdate()
    {
        LimitSpeed();
        LookRotation();
        Move();
    }

    public void ApplyForce(Vector2 force)
    {
        if (force == Vector2.zero) return;
        
        Velocity += force * Time.fixedDeltaTime;
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
        transform.position += (Vector3)Velocity * Time.fixedDeltaTime;
    }

    void LookRotation()
    {
        if (Velocity.sqrMagnitude > 0.0001f) // tránh zero vector
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(new Vector3(Velocity.x, Velocity.y, 0)),
                0.3f);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (settings == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, settings.range);
        
        Gizmos.color = Color.white;
        Gizmos.DrawLine(
            transform.position,
            transform.position + ((Vector3)Velocity.normalized * Velocity.magnitude)
        );
    }
}
