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

    protected override void CaculateBoids()
    {
        // 🔹 lấy bounds theo custom hay camera
        Rect worldBounds = useCustomBounds
            ? customBounds
            : new Rect(-20, -20, 40, 40); 

        root?.ReleaseAll(quadTreePool);
        root = quadTreePool.Get().Init(worldBounds, 8, 1f, 0, 8);
        foreach (BoidScript b in listBoid)
            root.Insert(b, quadTreePool);

        foreach (BoidScript boid in listBoid)
        {
            Vector2 separation = Vector2.zero;
            Vector2 alignment = Vector2.zero;
            Vector2 cohesion = Vector2.zero;
            int sepCount = 0, alignCount = 0, cohesionCount = 0;
            // Query neighbors trong quadtree
            List<BoidScript> neighbors = new();
            Rect queryRange = new Rect(
                boid.Position.x - settings.cohesionRange,
                boid.Position.y - settings.cohesionRange,
                settings.cohesionRange * 2,
                settings.cohesionRange * 2
            );
            root.QueryRange(queryRange, neighbors);

            foreach (BoidScript other in neighbors)
            {
                if (other == boid) continue;

                float dist = Vector2.Distance(boid.Position, other.Position);
                if (dist < settings.separationRange && dist > 0f)
                {
                    Vector2 dir = (boid.Position - other.Position).normalized;
                    separation += dir * settings.separationRange / dist;
                    sepCount++;
                }

                if (dist < settings.alignmentRange)
                {
                    alignment += other.Velocity;
                    alignCount++;
                }

                if (dist < settings.cohesionRange)
                {
                    cohesion += other.Position;
                    cohesionCount++;
                }
            }

            if (sepCount > 0) separation /= sepCount;
            if (alignCount > 0) alignment /= alignCount;
            if (cohesionCount > 0) cohesion = cohesion / cohesionCount - boid.Position;

            Vector2 force = Vector2.zero;
            if (separation != Vector2.zero) force += separation * settings.separationWeight;
            if (alignment != Vector2.zero) force += alignment * settings.alignWeight;
            if (cohesion != Vector2.zero) force += cohesion * settings.cohesionWeight;
            force += cameraBounds.KeepWithinBounds(boid.Position) * settings.BoundWeight;
            boid.ApplyForce(force);
        }
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
