using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    private readonly Camera cam;
    private readonly float margin;

    public CameraBounds(Camera cam, float margin = 5f)
    {
        this.cam = cam;
        this.margin = margin;;
    }

public Vector2 KeepWithinBounds(Vector2 pos)
{
    Vector2 min = cam.ViewportToWorldPoint(new Vector3(0, 0));
    Vector2 max = cam.ViewportToWorldPoint(new Vector3(1, 1));
    Vector2 direction = Vector2.zero;

    if (pos.x < min.x + margin) direction.x = 1;
    if (pos.x > max.x - margin) direction.x = -1;
    if (pos.y < min.y + margin) direction.y = 1;
    if (pos.y > max.y - margin) direction.y = -1;

    return direction.normalized;
}

    public void DrawGizmos()
    {
        Vector2 min = cam.ViewportToWorldPoint(new Vector3(0, 0));
        Vector2 max = cam.ViewportToWorldPoint(new Vector3(1, 1));

        min += Vector2.one * margin;
        max -= Vector2.one * margin;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((min + max) / 2f, max - min);
    }
}
