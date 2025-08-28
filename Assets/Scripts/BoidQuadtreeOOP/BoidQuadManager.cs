using System.Collections.Generic;
using UnityEngine;

public class BoidQuadTreeManager : MonoBehaviour
{
    public static BoidQuadTreeManager Instance { get; private set; }

    [Header("Boid Settings")]
    [SerializeField] int boidCount = 100;
    [SerializeField] GameObject boidPrefab;
    [SerializeField] BoidSettings boidSettings;
    public Rect quadTreeBound;

    public List<BoidScript> listBoid;
    private CameraBounds cameraBounds;
    private QuadTree quadTree;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        cameraBounds = gameObject.AddComponent<CameraBounds>();
        cameraBounds.Initialize(Camera.main, 5f);

        listBoid = new List<BoidScript>(boidCount);

        quadTree = new QuadTree(quadTreeBound, capacity: 6, minSize: 1, maxDepth: 7);

        SpawnBoids();
    }

void FixedUpdate()
{
    // 1. Clear & build quadtree
    quadTree.Clear();
    for (int i = 0; i < listBoid.Count; i++)
        quadTree.Insert(i, listBoid[i].Position);

    // 2. Duyệt từng boid
    for (int i = 0; i < listBoid.Count; i++)
    {
        BoidScript boid = listBoid[i];

        Vector2 separation = Vector2.zero;
        Vector2 alignment = Vector2.zero;
        Vector2 cohesion = Vector2.zero;
        int sepCount = 0, alignCount = 0, cohesionCount = 0;

        // Query trong bounding box = max range
        float queryRange = Mathf.Max(
            boidSettings.separationRange,
            boidSettings.alignmentRange,
            boidSettings.cohesionRange
        );

        Rect range = new Rect(
            boid.Position - Vector2.one * queryRange,
            Vector2.one * queryRange * 2
        );

        // Dùng buffer chung để giảm GC
        _neighborBuffer.Clear();
        quadTree.QueryRange(range, _neighborBuffer);

        foreach (int idx in _neighborBuffer)
        {
            if (idx == i) continue; // bỏ chính nó
            BoidScript other = listBoid[idx];

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

        // Chuẩn hoá
        if (sepCount > 0) separation /= sepCount;
        if (alignCount > 0) alignment /= alignCount;
        if (cohesionCount > 0) cohesion = (cohesion / cohesionCount) - boid.Position;

        // Tổng lực
        Vector2 force = Vector2.zero;
        if (sepCount > 0) force += separation * boidSettings.separationWeight;
        if (alignCount > 0) force += alignment * boidSettings.alignWeight;
        if (cohesionCount > 0) force += cohesion * boidSettings.cohesionWeight;

        // Lực giữ trong camera
        force += cameraBounds.KeepWithinBounds(boid.Position) * boidSettings.BoundWeight;

        boid.ApplyForce(force);
    }
}

// Buffer chung để tránh new List mỗi vòng
private static readonly List<int> _neighborBuffer = new List<int>(256);

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
        if (quadTree != null)
        {
            quadTree.DrawGizmos();
        }
        if (cameraBounds != null)
        {
            cameraBounds.DrawGizmos();
        }
    }
}
