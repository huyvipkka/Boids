using System.Collections.Generic;
using UnityEngine;

public class QuadTreeNode
{
    public Rect Bounds { get; private set; }
    private int _capacity;
    private float _minSize;
    private int _depth;
    private int _maxDepth;

    private List<int> _points;
    private QuadTreeNode[] _children;
    public bool IsDivided => _children != null;

    public void Init(Rect bounds, int capacity, float minSize, int depth, int maxDepth)
    {
        Bounds = bounds;
        _capacity = capacity;
        _minSize = minSize;
        _depth = depth;
        _maxDepth = maxDepth;

        _points ??= new List<int>(_capacity);
        _points.Clear();
        _children = null;
    }

    public bool Insert(int index, Vector2 point, QuadTreeNodePool pool)
    {
        if (!Bounds.Contains(point))
            return false;

        if (_points.Count < _capacity || !CanDivide())
        {
            _points.Add(index);
            return true;
        }

        if (!IsDivided)
        {
            Subdivide(pool);

            // phân phối lại các point cũ xuống con
            for (int i = _points.Count - 1; i >= 0; i--)
            {
                int idx = _points[i];
                Vector2 oldPoint = pool.AllPoints[idx]; // Pool giữ reference allPoints
                foreach (var child in _children)
                {
                    if (child.Insert(idx, oldPoint, pool))
                        break;
                }
            }
            _points.Clear();
        }

        foreach (var child in _children)
        {
            if (child.Insert(index, point, pool))
                return true;
        }

        return false;
    }

    public void QueryRange(Rect range, IList<Vector2> allPoints, List<int> found)
    {
        if (!Bounds.Overlaps(range, true))
            return;

        foreach (var idx in _points)
        {
            if (range.Contains(allPoints[idx]))
                found.Add(idx);
        }

        if (IsDivided)
        {
            foreach (var child in _children)
                child.QueryRange(range, allPoints, found);
        }
    }

    private bool CanDivide()
    {
        return _depth < _maxDepth
               && (Bounds.width / 2 >= _minSize && Bounds.height / 2 >= _minSize);
    }

    private void Subdivide(QuadTreeNodePool pool)
    {
        Vector2 half = new(Bounds.width / 2f, Bounds.height / 2f);

        Rect nw = new(new Vector2(Bounds.x, Bounds.y + half.y), half);
        Rect ne = new(new Vector2(Bounds.x + half.x, Bounds.y + half.y), half);
        Rect sw = new(new Vector2(Bounds.x, Bounds.y), half);
        Rect se = new(new Vector2(Bounds.x + half.x, Bounds.y), half);

        _children = new QuadTreeNode[4];
        _children[0] = pool.Get(nw, _depth + 1);
        _children[1] = pool.Get(ne, _depth + 1);
        _children[2] = pool.Get(sw, _depth + 1);
        _children[3] = pool.Get(se, _depth + 1);
    }

    public void DrawGizmo()
    {
        Gizmos.color = Color.green;
        Vector3 center = new(Bounds.center.x, Bounds.center.y, 0f);
        Vector3 size = new(Bounds.size.x, Bounds.size.y, 0f);
        Gizmos.DrawWireCube(center, size);

        if (IsDivided)
        {
            foreach (var child in _children)
                child.DrawGizmo();
        }
    }
    public void ReleaseChildren(QuadTreeNodePool pool)
    {
        if (_children != null)
        {
            foreach (var child in _children)
                pool.ReleaseRecursive(child);
            _children = null;
        }
    }
}