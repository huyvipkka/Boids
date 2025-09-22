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
    [SerializeField] protected MonoBehaviour boundComponent;

    // Thuộc tính để lấy interface
    protected IBoundary bound => boundComponent as IBoundary;
    protected Rect BoundSize => bound.GetBoundSize();

    [SerializeField] protected bool autoAddBoid = false;
    [SerializeField] protected int boidStep = 10;
    [SerializeField] protected int timeSpawn = 5;

    protected virtual void Start()
    {
        listBoid = new List<BoidScript>(boidCount);
        Spawn();
        Debug.Log("BoidManager.Start");
    }

    protected virtual void Update()
    {
        foreach (BoidScript boid in listBoid)
        {
            List<BoidScript> neighbors = FindNeighbors(boid);
            Vector2 force = CalculateForces(boid, neighbors);
            boid.ApplyForce(force);
        }
        AutoAddBoid();
    }

    protected float timer;
    protected virtual void AutoAddBoid()
    {
        if (!autoAddBoid) return;
        timer += Time.deltaTime;
        if (timer >= 10f)
        {
            timer = 0f;
            AddBoids();
        }
    }

    protected virtual List<BoidScript> FindNeighbors(BoidScript boid)
    {
        List<BoidScript> neighbors = new();
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

        if (force.magnitude > settings.maxSteerForce)
            force = force.normalized * settings.maxSteerForce;
        force += bound.GetForce(boid.Position) * settings.BoundWeight;

        return force;
    }

    protected virtual void Spawn()
    {
        Rect r = new(
            -10f, -10f, 20, 20
        );

        for (int i = 0; i < boidCount; i++)
        {
            float x = UnityEngine.Random.Range(r.xMin, r.xMax);
            float y = UnityEngine.Random.Range(r.yMin, r.yMax);
            Vector2 pos = new Vector2(x, y);

            GameObject obj = Instantiate(boidPrefab, pos, Quaternion.identity);
            BoidScript b = obj.GetComponent<BoidScript>();
            b.Velocity = UnityEngine.Random.insideUnitCircle;
            listBoid.Add(b);
        }
    }
    protected void AddBoids()
    {
        Rect r = BoundSize;

        for (int i = 0; i < boidStep; i++)
        {
            float x = UnityEngine.Random.Range(r.xMin, r.xMax);
            float y = UnityEngine.Random.Range(r.yMin, r.yMax);
            Vector2 pos = new Vector2(x, y);

            GameObject obj = Instantiate(boidPrefab, pos, Quaternion.identity);
            BoidScript b = obj.GetComponent<BoidScript>();
            b.Velocity = UnityEngine.Random.insideUnitCircle;
            listBoid.Add(b);
        }
    }

    protected virtual void OnDrawGizmos()
    {
        bound?.DrawGizmos();
    }
}