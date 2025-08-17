using System.Collections.Generic;
using UnityEngine;

public class BoidScript : MonoBehaviour
{
    public Vector2 Position => transform.position;
    public Vector2 Velocity;
    [SerializeField] BoidSettings settings;

    public void ApplyForce(Vector2 force)
    {
        Debug.Log(force.magnitude);
        Velocity += force * Time.fixedDeltaTime;
        LimitSpeed();
        LookRotation();
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


    public void Move()
    {
        transform.position += (Vector3)Velocity * Time.fixedDeltaTime;
    }

    void LookRotation()
    {
        transform.rotation = Quaternion.Slerp(transform.localRotation,
            Quaternion.LookRotation(Velocity), 0.3f);
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
