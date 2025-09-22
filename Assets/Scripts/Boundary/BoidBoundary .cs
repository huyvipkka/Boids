using UnityEngine;

public class BoidBoundary : MonoBehaviour, IBoundary
{
    public static BoidBoundary Instance { get; private set; }

    [Header("Boundary Settings")]
    public float boundaryRadius = 30f;
    public float maxBoundaryWeight = 0.5f;
    // boid trong vùng này thì không chịu lực
    [Range(0f, 1f)] public float innerRadiusFactor = 0.5f;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Vector2 GetForce(Vector2 boidPos)
    {
        Vector2 toCenter = (Vector2)transform.position - boidPos;
        float distFromCenter = toCenter.magnitude;

        if (distFromCenter < boundaryRadius * innerRadiusFactor)
            return Vector2.zero; 

        // t = 0 khi ở innerRadiusFactor * boundaryRadius, t = 1 khi ở boundaryRadius
        float t = Mathf.InverseLerp(boundaryRadius * innerRadiusFactor, boundaryRadius, distFromCenter);
        t = Mathf.Clamp01(t);

        return toCenter.normalized * (t * maxBoundaryWeight);
    }

    
    public void DrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, boundaryRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, boundaryRadius * innerRadiusFactor);
    }

    public Rect GetBoundSize()
    {
        float innerRadius = boundaryRadius * innerRadiusFactor;
        float size = innerRadius * 2f;

        Vector2 center = transform.position;
        return new Rect(center.x - innerRadius, center.y - innerRadius, size, size);
    }
}
