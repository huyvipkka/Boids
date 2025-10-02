using System.Collections.Generic;
using UnityEngine;

public class BoidQuadTreeManager : BoidManager
{

    [Header("QuadTree Bounds")]
    [SerializeField] float expandFactor = 0.2f;

    private ObjectPool<QuadTree<BoidScript>> quadTreePool;
    private QuadTree<BoidScript> root;

    protected override void Start()
    {
        base.Start();
        quadTreePool = new ObjectPool<QuadTree<BoidScript>>(
            () => new QuadTree<BoidScript>(b => b.Position), 50, 500
        );
    }

    protected override void Update()
    {
        ResetQuadtree();
        base.Update();
    }

    protected override List<BoidScript> FindNeighbors(BoidScript boid)
    {
        List<BoidScript> neighbors = new();
        Rect queryRange = new(
            boid.Position.x - settings.range,
            boid.Position.y - settings.range,
            settings.range * 2,
            settings.range * 2
        );
        root.QueryRange(queryRange, neighbors);
        return neighbors;
    }

    private void ResetQuadtree()
    {
        Vector2 center = BoundSize.center;
        Vector2 newSize = BoundSize.size * (1 + expandFactor);
        Rect QuadTreeRect = new(
            center.x - newSize.x / 2f,
            center.y - newSize.y / 2f,
            newSize.x,
            newSize.y
        );

        root?.ReleaseAll(quadTreePool);
        root = quadTreePool.Get().Init(QuadTreeRect, 8, 1f, 0, 8);

        foreach (BoidScript b in listBoid)
            root.Insert(b, quadTreePool);
    }


    protected override void OnDrawGizmos()
    {
        root?.DrawGizmos();
        base.OnDrawGizmos();
    }
}
