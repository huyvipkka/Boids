using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class JBOLManager : BoidManager
{
    // NativeArrays để xử lý song song
    private NativeArray<float2> positions;
    private NativeArray<float2> velocities;
    private NativeArray<float2> forces;
    protected override void Start()
    {
        base.Start();

        positions = new NativeArray<float2>(boidCount, Allocator.Persistent);
        velocities = new NativeArray<float2>(boidCount, Allocator.Persistent);
        forces = new NativeArray<float2>(boidCount, Allocator.Persistent);
    }
    protected override void Update()
    {
        for (int i = 0; i < listBoid.Count; i++)
        {
            positions[i] = listBoid[i].Position;
            velocities[i] = listBoid[i].Velocity;
        }
        BoidSettingsData settingsData = new()
        {
            range = settings.range,
            separationWeight = settings.separationWeight,
            alignWeight = settings.alignWeight,
            cohesionWeight = settings.cohesionWeight
        };
        BoidJob job = new()
        {
            positions = positions,
            velocities = velocities,
            forces = forces,
            settings = settingsData
        };
        JobHandle handle = job.Schedule(listBoid.Count, 32);
        handle.Complete();
        for (int i = 0; i < listBoid.Count; i++)
        {
            //Debug.Log(forces[i]);
            float2 boundForce = (float2)bound.GetForce(listBoid[i].Position) * settings.BoundWeight;
            listBoid[i].ApplyForce(forces[i] + boundForce);
        }

        AutoAddBoid();
    }
    void OnDestroy()
    {
        if (positions.IsCreated) positions.Dispose();
        if (velocities.IsCreated) velocities.Dispose();
        if (forces.IsCreated) forces.Dispose();
    }
}