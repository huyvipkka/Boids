using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    public Camera cam { get; private set; }
    private float margin;
    private static Vector3 vt11 = new(1, 1, 0);
    public void Initialize(Camera cam, float margin = 5f)
    {
        this.cam = cam;
        this.margin = margin;
    }

    public Vector2 KeepWithinBounds(Vector2 pos)
    {
        Vector2 min = cam.ViewportToWorldPoint(Vector3.zero);
        Vector2 max = cam.ViewportToWorldPoint(vt11);
        Vector2 direction = Vector2.zero;

        if (pos.x < min.x + margin) direction.x = 1;
        if (pos.x > max.x - margin) direction.x = -1;
        if (pos.y < min.y + margin) direction.y = 1;
        if (pos.y > max.y - margin) direction.y = -1;

        return direction.normalized;
    }
    public void DrawGizmos()
    {
        Vector2 min = cam.ViewportToWorldPoint(Vector3.zero);
        Vector2 max = cam.ViewportToWorldPoint(vt11);

        min += Vector2.one * margin;
        max -= Vector2.one * margin;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((min + max) / 2f, max - min);
    }
}
