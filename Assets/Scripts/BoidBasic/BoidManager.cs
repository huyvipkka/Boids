using System.Collections.Generic;
using Unity.Mathematics;
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

    protected virtual void Update()
    {
        foreach (BoidScript boid in listBoid)
        {
            List<BoidScript> neighbors = FindNeighbors(boid);
            Vector2 force = CalculateForces(boid, neighbors);
            boid.ApplyForce(force);
        }
    }

    protected virtual List<BoidScript> FindNeighbors(BoidScript boid)
    {
        List<BoidScript> neighbors = new List<BoidScript>();

        foreach (BoidScript other in listBoid)
        {
            if (other == boid) continue;
            float dist = Vector2.Distance(boid.Position, other.Position);
            if (dist < settings.range && dist > 0f)
            {
                neighbors.Add(other);
            }
        }

        return neighbors;
    }

    protected virtual Vector2 CalculateForces(BoidScript boid, List<BoidScript> neighbors)
    {
        Vector2 separation = Vector2.zero;
        Vector2 alignment = Vector2.zero;
        Vector2 cohesion = Vector2.zero;

        int sepCount = 0, alignCount = 0, cohesionCount = 0;

        foreach (BoidScript other in neighbors)
        {
            float dist = Vector2.Distance(boid.Position, other.Position);
            dist = math.max(0.01f, dist);

            Vector2 dir = (boid.Position - other.Position).normalized;
            separation += dir * settings.range / dist;
            sepCount++;

            alignment += other.Velocity;
            alignCount++;

            cohesion += other.Position;
            cohesionCount++;
        }

        if (sepCount > 0) separation /= sepCount;
        if (alignCount > 0) alignment /= alignCount;
        if (cohesionCount > 0) cohesion = cohesion / cohesionCount - boid.Position;

        Vector2 force = Vector2.zero;
        if (sepCount > 0) force += separation * settings.separationWeight;
        if (alignCount > 0) force += alignment * settings.alignWeight;
        if (cohesionCount > 0) force += cohesion * settings.cohesionWeight;
        force += cameraBounds.KeepWithinBounds(boid.Position) * settings.BoundWeight;

        return force;
    }

    protected virtual void SpawnOnCircle()
    {
        for (int i = 0; i < boidCount; i++)
        {
            Vector2 pos = UnityEngine.Random.insideUnitCircle * 20f;
            GameObject obj = Instantiate(boidPrefab, pos, Quaternion.identity);
            BoidScript b = obj.GetComponent<BoidScript>();
            b.Velocity = UnityEngine.Random.insideUnitCircle * 2f;
            listBoid.Add(b);
        }
    }

    public virtual void AddBoids(int n)
    {
        for (int i = 0; i < n; i++)
        {
            Vector2 pos = UnityEngine.Random.insideUnitCircle * 20f;
            GameObject obj = Instantiate(boidPrefab, pos, Quaternion.identity);
            BoidScript b = obj.GetComponent<BoidScript>();
            b.Velocity = UnityEngine.Random.insideUnitCircle * 2f;
            listBoid.Add(b);
        }
    }

    protected virtual void OnDrawGizmos()
    {
        cameraBounds?.DrawGizmos();
    }
}
