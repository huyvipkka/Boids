using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [Header("Boid Settings")]
    [SerializeField] protected int boidCount = 20;
    [SerializeField] protected GameObject boidPrefab;
    [SerializeField] protected BoidSettings settings;
    public List<BoidScript> listBoid;
    public CameraBounds cameraBounds;

    protected virtual void Start()
    {
        cameraBounds = gameObject.AddComponent<CameraBounds>();
        cameraBounds.Initialize(Camera.main, 5f);
        listBoid = new List<BoidScript>(boidCount);
        SpawnOnCircle();
    }

    void FixedUpdate()
    {
        CaculateBoids();
    }

    protected virtual void CaculateBoids() {
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
                    Vector2 dir = (boid.Position - other.Position).normalized;
                    separation += dir * settings.separationRange / dist;
                    sepCount++;
                }
                // Alignment
                if (dist < settings.alignmentRange)
                {
                    alignment += other.Velocity; 
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

    protected virtual void SpawnOnCircle()
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

    public virtual void AddBoids(int n)
    {
        for (int i = 0; i < n; i++)
        {
            Vector2 pos = Random.insideUnitCircle * 20f;
            GameObject obj = Instantiate(boidPrefab, pos, Quaternion.identity);
            BoidScript b = obj.GetComponent<BoidScript>();
            b.Velocity = Random.insideUnitCircle * 2f;
            listBoid.Add(b);
        }
    }

    protected virtual void OnDrawGizmos()
    {
        cameraBounds?.DrawGizmos();
    }
}
