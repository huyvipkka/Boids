using UnityEngine;

public class DensityBound : MonoBehaviour, IBoundary
{
    [Tooltip("Số lượng boid trung bình / 100 đơn vị diện tích")]
    [SerializeField] float density = 10;

    [SerializeField] BoidManager manager;

    private float halfSize;

    void Update()
    {
        AutoResize();
    }

    void AutoResize()
    {
        int boidCount = manager.listBoid.Count;
        if (boidCount == 0) return;
        float area = boidCount * 100 / density;
        float side = Mathf.Sqrt(area);

        halfSize = side / 2f;
    }

    public Vector2 GetForce(Vector2 boidPos)
    {
        Vector2 force = Vector2.zero;
        Vector2 localPos = boidPos - (Vector2)transform.position;

        if (Mathf.Abs(localPos.x) > halfSize)
        {
            float dir = Mathf.Sign(localPos.x);
            force.x = -dir;
        }

        if (Mathf.Abs(localPos.y) > halfSize)
        {
            float dir = Mathf.Sign(localPos.y);
            force.y = -dir;
        }

        return force;
    }

    public void DrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(halfSize * 2, halfSize * 2, 0));
    }
    
    public Rect GetBoundSize()
    {
        Vector2 center = transform.position;
        return new Rect(
            center.x - halfSize,
            center.y - halfSize,
            halfSize * 2f,
            halfSize * 2f
        );
    }

}
