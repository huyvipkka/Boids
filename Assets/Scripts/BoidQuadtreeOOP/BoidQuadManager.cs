using System.Collections.Generic;
using UnityEngine;

public class BoidQuadTreeManager : BoidManager
{

    [Header("QuadTree Bounds")]
    [SerializeField] bool useCustomBounds = false;
    [SerializeField] Rect customBounds = new(-20, -20, 40, 40);

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
        Rect worldBounds = useCustomBounds
            ? customBounds
            : new Rect(-20, -20, 40, 40); 

        root?.ReleaseAll(quadTreePool);
        root = quadTreePool.Get().Init(worldBounds, 8, 1f, 0, 8);
        foreach (BoidScript b in listBoid)
            root.Insert(b, quadTreePool);
    }

    protected override void OnDrawGizmos()
    {
        if (useCustomBounds)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(customBounds.center, customBounds.size);
        }
        root?.DrawGizmos(Color.cyan);
        base.OnDrawGizmos();
    }
}
