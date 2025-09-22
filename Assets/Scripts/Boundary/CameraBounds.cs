using UnityEngine;

public class CameraBounds : MonoBehaviour, IBoundary
{
    public Camera cam { get; private set; }
    [SerializeField] float margin = 3;

    private static Vector3 vt11 = new(1, 1, 0);

    private Rect boundRect;

    void Start()
    {
        cam = Camera.main;
        UpdateBound();
    }

    void LateUpdate()
    {
        UpdateBound();
    }

    private void UpdateBound()
    {
        Vector2 min = cam.ViewportToWorldPoint(Vector3.zero);
        Vector2 max = cam.ViewportToWorldPoint(vt11);

        min += Vector2.one * margin;
        max -= Vector2.one * margin;

        boundRect = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    }

    public Rect GetBoundSize()
    {
        return boundRect;
    }

    public Vector2 GetForce(Vector2 pos)
    {
        Vector2 direction = Vector2.zero;

        if (pos.x < boundRect.xMin) direction.x = 1;
        if (pos.x > boundRect.xMax) direction.x = -1;
        if (pos.y < boundRect.yMin) direction.y = 1;
        if (pos.y > boundRect.yMax) direction.y = -1;

        return direction.normalized;
    }

    public void DrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boundRect.center, boundRect.size);
    }
}
