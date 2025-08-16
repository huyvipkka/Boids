using UnityEngine;

public class BoidBoundary : MonoBehaviour
{
    public static BoidBoundary Instance { get; private set; }

    [Header("Boundary Settings")]
    public float boundaryRadius = 30f;
    public float maxBoundaryWeight = 0.5f;
    [Range(0f, 1f)] public float innerRadiusFactor = 0.5f;
    // boid trong vùng này thì không chịu lực

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Vector3 GetBoundaryWeight(Vector3 boidPos)
    {
        Vector3 toCenter = transform.position - boidPos;
        float distFromCenter = toCenter.magnitude;

        if (distFromCenter < boundaryRadius * innerRadiusFactor)
            return Vector3.zero; 

        // t = 0 khi ở innerRadiusFactor * boundaryRadius, t = 1 khi ở boundaryRadius
        float t = Mathf.InverseLerp(boundaryRadius * innerRadiusFactor, boundaryRadius, distFromCenter);
        t = Mathf.Clamp01(t);

        return toCenter.normalized * (t * maxBoundaryWeight);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, boundaryRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, boundaryRadius * innerRadiusFactor);
    }
}
