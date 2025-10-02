using UnityEngine;

public class CameraBounds : MonoBehaviour, IBoundary
{
    public Camera cam { get; private set; }
    [SerializeField] private float margin;

    public void Start()
    {
        this.cam = Camera.main;
    }

    public Rect GetBoundSize()
    {
        float zDist = Mathf.Abs(cam.transform.position.z);
        Vector3 bl = cam.ViewportToWorldPoint(new Vector3(0f, 0f, zDist));
        Vector3 tr = cam.ViewportToWorldPoint(new Vector3(1f, 1f, zDist));
        Vector2 min = (Vector2)bl + Vector2.one * margin;
        Vector2 max = (Vector2)tr - Vector2.one * margin;
        return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    }

    public Vector2 GetForce(Vector2 pos)
    {
        Rect bound = GetBoundSize();
        if (bound.Contains(pos))
            return Vector2.zero;
        float clampedX = Mathf.Clamp(pos.x, bound.xMin, bound.xMax);
        float clampedY = Mathf.Clamp(pos.y, bound.yMin, bound.yMax);
        Vector2 nearest = new(clampedX, clampedY);

        return (nearest - pos).normalized;
    }

    public void DrawGizmos()
    {
        Rect bound = GetBoundSize();
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bound.center, bound.size);
    }

    private void OnDrawGizmosSelected()
    {
        if (cam == null) cam = Camera.main;
        DrawGizmos();
    }
}
