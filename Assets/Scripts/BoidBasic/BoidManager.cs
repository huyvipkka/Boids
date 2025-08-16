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
    [SerializeField] float spawnRadius = 2f;
    [SerializeField] BoidSettings settings;

    public List<BoidScript> listBoid;

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
        listBoid = new List<BoidScript>(boidCount);
        SpawnOnCircle();
    }

    void FixedUpdate()
    {
        foreach (BoidScript boid in listBoid)
        {
            Vector2 sepSum = Vector2.zero;
            Vector2 alignSum = Vector2.zero;
            Vector2 cohesionSum = Vector2.zero;
            int sepCount = 0, alignCount = 0, cohesionCount = 0;

            foreach (BoidScript other in listBoid)
            {
                if (other == boid) continue;

                float dist = Vector2.Distance(boid.Position, other.Position);


                if (dist < settings.separationRange)
                {
                    sepSum += (Vector2)(transform.position - other.transform.position) / dist;
                }

                // Alignment
                if (dist < settings.alignmentRange)
                {
                    alignSum += other.Velocity; // Lấy hướng thay vì vận tốc tuyệt đối
                    alignCount++;
                }

                // Cohesion
                if (dist < settings.cohesionRange)
                {
                    cohesionSum += other.Position;
                    cohesionCount++;
                }
            }
            Vector2 separation = Vector2.zero;
            Vector2 alignment = Vector2.zero;
            Vector2 cohesion = Vector2.zero;

            if (sepCount > 0) separation = sepSum.normalized;
            if (alignCount > 0) alignment = (alignSum / alignCount).normalized;
            if (cohesionCount > 0) cohesion = (cohesionSum / cohesionCount - boid.Position).normalized;
            // Tổng hợp các lực với trọng số
            Vector2 force = Vector2.zero;
            force += separation * settings.separationWeight;
            force += alignment * settings.alignWeight;
            force += cohesion * settings.cohesionWeight;
            force += (Vector2)BoidBoundary.Instance.GetBoundaryWeight(boid.Position);
            
            boid.ApplyForce(force);
        }
    }

    void SpawnOnCircle()
    {
        for (int i = 0; i < boidCount; i++)
        {
            float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            Vector2 dir = new(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 pos2D = dir * spawnRadius;

            float angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotation = Quaternion.Euler(0f, 0f, angleDeg);

            GameObject boidObj = Instantiate(boidPrefab, pos2D, rotation);
            BoidScript boidScript = boidObj.GetComponent<BoidScript>();
            listBoid.Add(boidScript);
        }
    }
}
