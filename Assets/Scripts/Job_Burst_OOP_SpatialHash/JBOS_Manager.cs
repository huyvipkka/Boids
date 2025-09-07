using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class JBOS_Manager : MonoBehaviour
{
    [SerializeField] int boidCount = 20;
    [SerializeField] GameObject boidPrefab;
    [SerializeField] BoidSettings settings;

    public List<BoidScript> listBoid;
    private CameraBounds cameraBounds;

    private NativeArray<float2> positions;
    private NativeArray<float2> velocities;
    private NativeArray<float2> forces;
    private NativeSpatialHash spatialHash;


    void Start()
    {
        cameraBounds = gameObject.AddComponent<CameraBounds>();
        cameraBounds.Initialize(Camera.main, 5f);

        listBoid = new List<BoidScript>(boidCount);
        SpawnOnCircle();

        positions = new NativeArray<float2>(boidCount, Allocator.Persistent);
        velocities = new NativeArray<float2>(boidCount, Allocator.Persistent);
        forces = new NativeArray<float2>(boidCount, Allocator.Persistent);
        float cellSize = Mathf.Max(settings.separationRange, settings.alignmentRange, settings.cohesionRange);
        spatialHash = new(boidCount * 2, cellSize, Allocator.Persistent);
    }

    void FixedUpdate()
    {

        spatialHash.Clear();
        for (int i = 0; i < listBoid.Count; i++)
        {
            positions[i] = listBoid[i].Position;
            velocities[i] = listBoid[i].Velocity;
            spatialHash.Insert(positions[i], i);
        }


        BoidSettingsData settingsData = new()
        {
            separationRange = settings.separationRange,
            alignmentRange = settings.alignmentRange,
            cohesionRange = settings.cohesionRange,
            separationWeight = settings.separationWeight,
            alignWeight = settings.alignWeight,
            cohesionWeight = settings.cohesionWeight
        };
        // float cellSize = Mathf.Max(settings.separationRange, settings.alignmentRange, settings.cohesionRange);
        // spatialHash.cellSize = cellSize;

        BoidHashJob job = new()
        {
            positions = positions,
            velocities = velocities,
            forces = forces,
            settings = settingsData,
            spatialMap = spatialHash

        };

        JobHandle handle = job.Schedule(listBoid.Count, 32);
        handle.Complete();

        for (int i = 0; i < listBoid.Count; i++)
        {
            float2 boundForce = (float2)cameraBounds.KeepWithinBounds(listBoid[i].Position) * settings.BoundWeight;
            listBoid[i].ApplyForce(forces[i] + boundForce);
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

    void OnDestroy()
    {
        if (positions.IsCreated) positions.Dispose();
        if (velocities.IsCreated) velocities.Dispose();
        if (forces.IsCreated) forces.Dispose();
        spatialHash.Dispose();
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            spatialHash.DrawGizmos();
            cameraBounds?.DrawGizmos();
        }

    }
}