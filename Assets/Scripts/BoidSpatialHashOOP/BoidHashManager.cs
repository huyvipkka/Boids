using System.Collections.Generic;
using UnityEngine;

public class BoidHashManager : MonoBehaviour
{
    // public static BoidHashManager Instance { get; private set; }

    [Header("Boid Settings")]
    [SerializeField] int boidCount = 100;
    [SerializeField] GameObject boidPrefab;
    [SerializeField] BoidSettings boidSettings;

    public List<BoidScript> listBoid;
    private CameraBounds cameraBounds;
    private SpatialHash<BoidScript> spatialHash;

    // void Awake()
    // {
    //     if (Instance != null && Instance != this)
    //     {
    //         Destroy(gameObject);
    //         return;
    //     }
    //     Instance = this;
    // }

    void Start()
    {
        cameraBounds = gameObject.AddComponent<CameraBounds>();
        cameraBounds.Initialize(Camera.main, 5f);

        listBoid = new List<BoidScript>(boidCount);
        spatialHash = new SpatialHash<BoidScript>(5, b => b.Position);
        SpawnBoids();
    }

    void FixedUpdate()
    {
        float cellSize = Mathf.Max( boidSettings.separationRange,
                                    boidSettings.alignmentRange,
                                    boidSettings.cohesionRange);
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

                if (dist < boidSettings.separationRange && dist > 0f)
                {
                    separation += (boid.Position - other.Position) / dist;
                    sepCount++;
                }
                if (dist < boidSettings.alignmentRange)
                {
                    alignment += other.Velocity;
                    alignCount++;
                }
                if (dist < boidSettings.cohesionRange)
                {
                    cohesion += other.Position;
                    cohesionCount++;
                }
            }

            if (sepCount > 0) separation /= sepCount;
            if (alignCount > 0) alignment /= alignCount;
            if (cohesionCount > 0) cohesion = (cohesion / cohesionCount) - boid.Position;

            Vector2 force = Vector2.zero;
            if (sepCount > 0) force += separation * boidSettings.separationWeight;
            if (alignCount > 0) force += alignment * boidSettings.alignWeight;
            if (cohesionCount > 0) force += cohesion * boidSettings.cohesionWeight;

            force += cameraBounds.KeepWithinBounds(boid.Position) * boidSettings.BoundWeight;
            boid.ApplyForce(force);
        }
    }

    void SpawnBoids()
    {
        for (int i = 0; i < boidCount; i++)
        {
            Vector2 pos = Random.insideUnitCircle * 20f;
            GameObject obj = Instantiate(boidPrefab, pos, Quaternion.identity);
            BoidScript b = obj.GetComponent<BoidScript>();
            b.Velocity = Random.insideUnitCircle * 2f;
            listBoid.Add(b);
        }
    }

    void OnDrawGizmos()
    {
        spatialHash?.DrawGizmos();
        cameraBounds?.DrawGizmos();
    }
}
