using System;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree<T>
{
    public Rect Bounds { get; private set; }
    private int _capacity;
    private float _minSize;
    private int _depth;
    private int _maxDepth;

    private QuadTree<T>[] _children;
    private List<T> _objects;
    public bool IsDivided => _children != null;

    private Func<T, Vector2> getPosition;
    public QuadTree()
    {
        _objects = new List<T>();
    }
    public QuadTree(Func<T, Vector2> getPosition)
    {
        this.getPosition = getPosition;
        _objects = new List<T>();
    }
    public QuadTree<T> Init(Rect bounds, int capacity, float minSize, int depth, int maxDepth, Func<T, Vector2> getPosition)
    {
        this.getPosition = getPosition;
        Bounds = bounds;
        _capacity = capacity;
        _minSize = minSize;
        _depth = depth;
        _maxDepth = maxDepth;

        _children = null;
        _objects.Clear();
        return this;
    }

    public QuadTree<T> Init(Rect bounds, int capacity, float minSize, int depth, int maxDepth)
    {
        Bounds = bounds;
        _capacity = capacity;
        _minSize = minSize;
        _depth = depth;
        _maxDepth = maxDepth;

        _children = null;
        _objects.Clear();
        return this;
    }

    public bool Insert(T item, ObjectPool<QuadTree<T>> pool)
    {
        Vector2 pos = getPosition(item);
        if (!Bounds.Contains(pos))
            return false;
        if (_objects.Count < _capacity || Mathf.Max(Bounds.width, Bounds.height) <= _minSize)
        {
            _objects.Add(item);
            return true;
        }

        // chia node nếu cần
        if (!IsDivided && _depth < _maxDepth)
            Subdivide(pool);

        if (IsDivided)
        {
            foreach (var child in _children)
            {
                if (child.Insert(item, pool))
                    return true;
            }
        }

        // fallback: giữ lại tại node hiện tại
        _objects.Add(item);
        return true;
    }
    private void Subdivide(ObjectPool<QuadTree<T>> pool)
    {
        float hw = Bounds.width / 2f;
        float hh = Bounds.height / 2f;
        float x = Bounds.x;
        float y = Bounds.y;

        _children = new QuadTree<T>[4];
        _children[0] = pool.Get().Init(new Rect(x, y, hw, hh), _capacity, _minSize, _depth + 1, _maxDepth);
        _children[1] = pool.Get().Init(new Rect(x + hw, y, hw, hh), _capacity, _minSize, _depth + 1, _maxDepth);
        _children[2] = pool.Get().Init(new Rect(x, y + hh, hw, hh), _capacity, _minSize, _depth + 1, _maxDepth);
        _children[3] = pool.Get().Init(new Rect(x + hw, y + hh, hw, hh), _capacity, _minSize, _depth + 1, _maxDepth);

        // phân phối lại object cũ xuống con
        foreach (var item in _objects)
        {
            Vector2 pos = getPosition(item);
            foreach (var child in _children)
            {
                if (child.Bounds.Contains(pos))
                {
                    child.Insert(item, pool);
                    break;
                }
            }
        }
        _objects.Clear();
    }

    public void QueryRange(Rect range, List<T> found)
    {
        if (!Bounds.Overlaps(range))
            return;

        foreach (var item in _objects)
        {
            Vector2 pos = getPosition(item);
            if (range.Contains(pos))
                found.Add(item);
        }

        if (IsDivided)
        {
            foreach (var child in _children)
                child.QueryRange(range, found);
        }
    }

    public void ReleaseAll(ObjectPool<QuadTree<T>> pool)
    {
        if (IsDivided)
        {
            foreach (var node in _children)
                node.ReleaseAll(pool);
        }
        _children = null;
        _objects.Clear();
        pool.Release(this);
    }

    public void DrawGizmos(Color? color = null)
    {
        Gizmos.color = color ?? Color.green;
        Gizmos.DrawWireCube(Bounds.center, Bounds.size);
        if (IsDivided)
        {
            foreach (var child in _children)
            {
                child?.DrawGizmos(color);
            }
        }
    }
}