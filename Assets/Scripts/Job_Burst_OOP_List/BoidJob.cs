using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct BoidJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<float2> positions;
    [ReadOnly] public NativeArray<float2> velocities;
    public NativeArray<float2> forces;

    public BoidSettingsData settings;

    public void Execute(int index)
    {
        float2 pos = positions[index];
        float2 vel = velocities[index];

        float2 separation = float2.zero;
        float2 alignment = float2.zero;
        float2 cohesion = float2.zero;
        int sepCount = 0, alignCount = 0, cohesionCount = 0;

        for (int j = 0; j < positions.Length; j++)
        {
            if (j == index) continue;
            float2 otherPos = positions[j];
            float dist = math.distance(pos, otherPos);

            if (dist < settings.separationRange && dist > 0f)
            {
                separation += (pos - otherPos) / dist;
                sepCount++;
            }
            if (dist < settings.alignmentRange)
            {
                alignment += velocities[j];
                alignCount++;
            }
            if (dist < settings.cohesionRange)
            {
                cohesion += otherPos;
                cohesionCount++;
            }
        }

        if (sepCount > 0) separation /= sepCount;
        if (alignCount > 0) alignment /= alignCount;
        if (cohesionCount > 0) cohesion = (cohesion / cohesionCount) - pos;

        float2 force = float2.zero;
        if (!separation.Equals(float2.zero)) force += separation * settings.separationWeight;
        if (!alignment.Equals(float2.zero)) force += alignment * settings.alignWeight;
        if (!cohesion.Equals(float2.zero)) force += cohesion * settings.cohesionWeight;

        forces[index] = force;
    }
}
