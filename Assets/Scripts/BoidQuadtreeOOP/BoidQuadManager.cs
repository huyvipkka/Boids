using System.Collections.Generic;
using UnityEngine;

public class BoidQuadTreeManager : MonoBehaviour
{
    [Header("Boid Settings")]
    [SerializeField] int boidCount = 20;
    [SerializeField] GameObject boidPrefab;
    [SerializeField] BoidSettings settings;

    [Header("QuadTree Bounds")]
    [SerializeField] bool useCustomBounds = false;
    [SerializeField] Rect customBounds = new Rect(-20, -20, 40, 40);

    public List<BoidScript> listBoid;
    private CameraBounds cameraBounds;
    private ObjectPool<QuadTree<BoidScript>> quadTreePool;
    private QuadTree<BoidScript> root;

    void Start()
    {
        listBoid = new List<BoidScript>(boidCount);
        SpawnOnCircle();

        quadTreePool = new ObjectPool<QuadTree<BoidScript>>(
            () => new QuadTree<BoidScript>(b => b.Position), 50, 500
        );

        cameraBounds = gameObject.AddComponent<CameraBounds>();
        cameraBounds.Initialize(Camera.main, 5f);
    }

    void FixedUpdate()
    {
        // 🔹 lấy bounds theo custom hay camera
        Rect worldBounds = useCustomBounds
            ? customBounds
            : new Rect(-20, -20, 40, 40); // fallback nếu không có cameraBounds


        root?.ReleaseAll(quadTreePool);
        root = quadTreePool.Get().Init(worldBounds, 8, 3f, 0, 8);
        foreach (BoidScript b in listBoid)
        {
            root.Insert(b, quadTreePool);
        }

        // 🔹 2. Tính toán lực cho từng boid bằng cách query vùng lân cận
        foreach (BoidScript boid in listBoid)
        {
            Vector2 separation = Vector2.zero;
            Vector2 alignment = Vector2.zero;
            Vector2 cohesion = Vector2.zero;
            int sepCount = 0, alignCount = 0, cohesionCount = 0;

            // Query neighbors trong quadtree
            List<BoidScript> neighbors = new List<BoidScript>();
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
            if (cohesionCount > 0) cohesion = cohesion / cohesionCount - boid.Position;

            Vector2 force = Vector2.zero;
            if (separation != Vector2.zero) force += separation * settings.separationWeight;
            if (alignment != Vector2.zero) force += alignment * settings.alignWeight;
            if (cohesion != Vector2.zero) force += cohesion * settings.cohesionWeight;
            force += cameraBounds.KeepWithinBounds(boid.Position) * settings.BoundWeight;

            boid.ApplyForce(force);
        }
    }

    void OnDrawGizmos()
    {
        if (useCustomBounds)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(customBounds.center, customBounds.size);
        }
        Debug.Log(root);
        root?.DrawGizmos(Color.cyan);
        cameraBounds?.DrawGizmos();
    }

    void SpawnOnCircle()
    {
        for (int i = 0; i < boidCount; i++)
        {
            Vector2 pos = UnityEngine.Random.insideUnitCircle * 10f;
            GameObject obj = Instantiate(boidPrefab, pos, Quaternion.identity);
            BoidScript b = obj.GetComponent<BoidScript>();
            b.Velocity = UnityEngine.Random.insideUnitCircle * 2f;
            listBoid.Add(b);
        }
    }
}
