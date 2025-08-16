using System.Collections.Generic;
using UnityEngine;

public class BoidScript : MonoBehaviour
{
    public Vector2 Position => transform.position;
    public Vector2 Velocity;
    public List<Vector2> history = new List<Vector2>();

    [SerializeField] BoidSettings settings;

    void Start()
    {
        Velocity = transform.up * settings.minSpeed;
    }

    public void ApplyForce(Vector2 force)
    {
        Velocity += force * Time.deltaTime;
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


public void Move()
{
    transform.position += (Vector3)Velocity * Time.deltaTime;

    // lưu lại lịch sử để debug / trail
    history.Add(transform.position);
    if (history.Count > 50)
        history.RemoveAt(0);

    // quay theo hướng bay
    if (Velocity != Vector2.zero)
    {
        float angle = Mathf.Atan2(Velocity.y, Velocity.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Euler(0, 0, angle);
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
