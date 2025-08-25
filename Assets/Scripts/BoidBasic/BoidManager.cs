using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public static BoidManager Instance { get; private set; }

    [Header("Boid Settings")]
    [SerializeField] int boidCount = 20;
    [SerializeField] GameObject boidPrefab;
    // [SerializeField] float spawnRadius = 2f;
    [SerializeField] BoidSettings settings;

    public List<BoidScript> listBoid;
    private CameraBounds cameraBounds;

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
        cameraBounds = new CameraBounds(Camera.main, 5f);
        listBoid = new List<BoidScript>(boidCount);
        SpawnOnCircle();
    }

    void FixedUpdate()
    {
        foreach (BoidScript boid in listBoid)
        {
            Vector2 separation = Vector2.zero;
            Vector2 alignment = Vector2.zero;
            Vector2 cohesion = Vector2.zero;
            int sepCount = 0, alignCount = 0, cohesionCount = 0;

            foreach (BoidScript other in listBoid)
            {
                if (other == boid) continue;

                float dist = Vector2.Distance(boid.Position, other.Position);
                if (dist < settings.separationRange && dist > 0f)
                {
                    separation += (boid.Position - other.Position) / dist;
                    sepCount++;
                }

                // Alignment
                if (dist < settings.alignmentRange)
                {
                    alignment += other.Velocity; // Lấy hướng thay vì vận tốc tuyệt đối
                    alignCount++;
                }

                // Cohesion
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

    void OnDrawGizmos()
    {
        cameraBounds.DrawGizmos();
    }
}
