using System.Collections.Generic;
using UnityEngine;

public class QuadTreeNodePool
{
    private readonly Stack<QuadTreeNode> _pool;
    private readonly int _capacity;
    private readonly float _minSize;
    private readonly int _maxDepth;

    public IList<Vector2> AllPoints; // giữ ref tới data points (tùy design)

    public QuadTreeNodePool(int capacity, float minSize, int maxDepth, int prewarmCount)
    {
        _capacity = capacity;
        _minSize = minSize;
        _maxDepth = maxDepth;
        _pool = new Stack<QuadTreeNode>(prewarmCount);

        for (int i = 0; i < prewarmCount; i++)
        {
            _pool.Push(new QuadTreeNode());
        }
    }

    public QuadTreeNode Get(Rect bounds, int depth)
    {
        QuadTreeNode node = _pool.Count > 0 ? _pool.Pop() : new QuadTreeNode();
        node.Init(bounds, _capacity, _minSize, depth, _maxDepth);
        return node;
    }

    public void ReleaseRecursive(QuadTreeNode node)
    {
        node.ReleaseChildren(this);
        _pool.Push(node);
    }
}