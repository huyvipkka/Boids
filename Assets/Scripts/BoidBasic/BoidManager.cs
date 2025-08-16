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
                    separation += (Vector2)(boid.transform.position - other.transform.position);
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


            if (sepCount > 0) separation = separation.normalized;
            if (alignment != Vector2.zero && alignCount > 0) alignment = alignment / alignCount - boid.Velocity;
            if (cohesionCount > 0) cohesion = cohesion / cohesionCount - boid.Position;
            // Tổng hợp các lực với trọng số
            Vector2 force = boid.Velocity;
            if (separation != Vector2.zero) force += separation * settings.separationWeight;
            if (alignment != Vector2.zero)  force += alignment * settings.alignWeight;
            if (cohesion != Vector2.zero)   force += cohesion * settings.cohesionWeight;
            force += cameraBounds.KeepWithinBounds(boid.Position) * settings.BoundWeight;
            
            boid.ApplyForce(force);
        }
    }

    void SpawnOnCircle()
    {
        // for (int i = 0; i < boidCount; i++)
        // {
        //     float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        //     Vector2 dir = new(Mathf.Cos(angle), Mathf.Sin(angle));
        //     Vector2 pos2D = dir * spawnRadius;

        //     float angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        //     Quaternion rotation = Quaternion.Euler(0f, 0f, angleDeg);

        //     GameObject boidObj = Instantiate(boidPrefab, pos2D, rotation);
        //     BoidScript boidScript = boidObj.GetComponent<BoidScript>();
        //     listBoid.Add(boidScript);
        // }

        for (int i = 0; i < boidCount; i++)
        {
            Vector2 pos = UnityEngine.Random.insideUnitCircle * 5f;
            GameObject obj = Instantiate(boidPrefab, pos, Quaternion.identity);
            BoidScript b = obj.GetComponent<BoidScript>();
            b.Velocity = UnityEngine.Random.insideUnitCircle * 2f;
            listBoid.Add(b);
        }
    }
}
