using UnityEngine;

public interface IBoundary
{
    public Vector2 GetForce(Vector2 boidPos);
    public Rect GetBoundSize();
    public void DrawGizmos();
}