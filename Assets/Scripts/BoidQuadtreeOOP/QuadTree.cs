using System.Collections.Generic;
using UnityEngine;

public class QuadTree
{
    private QuadTreeNode _root;
    private QuadTreeNodePool _pool;

    public Rect Bounds { get; }
    public int NodeCapacity { get; }
    public float NodeMinSize { get; }
    public int MaxDepth { get; }

    public QuadTree(Rect bounds, int capacity, float minSize, int maxDepth, int prewarmCount = 256)
    {
        Bounds = bounds;
        NodeCapacity = capacity;
        NodeMinSize = minSize;
        MaxDepth = maxDepth;

        // Khởi tạo pool trước
        _pool = new QuadTreeNodePool(capacity, minSize, maxDepth, prewarmCount);
        _root = _pool.Get(bounds, 0);
    }

    public void Clear()
    {
        _pool.ReleaseRecursive(_root);
        _root = _pool.Get(Bounds, 0);
    }

    public void Insert(int index, Vector2 position)
    {
        _root.Insert(index, position, _pool);
    }

    public List<int> QueryRange(Rect range, IList<Vector2> allPoints)
    {
        List<int> found = new();
        _root.QueryRange(range, allPoints, found);
        return found;
    }

    public void DrawGizmo()
    {
        _root.DrawGizmo();
    }
}
