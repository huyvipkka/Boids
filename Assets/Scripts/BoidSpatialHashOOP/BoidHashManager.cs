using System.Collections.Generic;
using UnityEngine;

public class BoidHashManager : BoidManager
{
    private SpatialHash<BoidScript> spatialHash;
    protected override void Start()
    {
        base.Start();
        spatialHash = new(5, b => b.Position);
    }
    protected override void CaculateBoids()
    {
        float cellSize = Mathf.Max( settings.separationRange,
                                    settings.alignmentRange,
                                    settings.cohesionRange);
        spatialHash.cellSize = cellSize;
        spatialHash.Clear();
        foreach (var boid in listBoid)
            spatialHash.Insert(boid);

        foreach (BoidScript boid in listBoid)
        {
            Vector2 separation = Vector2.zero;
            Vector2 alignment = Vector2.zero;
            Vector2 cohesion = Vector2.zero;
            int sepCount = 0, alignCount = 0, cohesionCount = 0;

            foreach (BoidScript other in spatialHash.GetNeighbors(boid.Position))
            {
                if (other == boid) continue;
                float dist = Vector2.Distance(boid.Position, other.Position);

                if (dist < settings.separationRange && dist > 0f)
                {
                    separation += (boid.Position - other.Position) / dist;
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
            if (cohesionCount > 0) cohesion = (cohesion / cohesionCount) - boid.Position;

            Vector2 force = Vector2.zero;
            if (sepCount > 0) force += separation * settings.separationWeight;
            if (alignCount > 0) force += alignment * settings.alignWeight;
            if (cohesionCount > 0) force += cohesion * settings.cohesionWeight;

            force += cameraBounds.KeepWithinBounds(boid.Position) * settings.BoundWeight;
            boid.ApplyForce(force);
        }
    }
    protected override void OnDrawGizmos()
    {
        spatialHash?.DrawGizmos();
        cameraBounds?.DrawGizmos();
    }
}
