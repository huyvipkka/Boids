using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct BoidHashJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<float2> positions;
    [ReadOnly] public NativeArray<float2> velocities;
    public NativeArray<float2> forces;

    [ReadOnly] public NativeSpatialHash spatialMap;
    public BoidSettingsData settings;

    public void Execute(int index)
    {
        float2 pos = positions[index];
        float2 separation = float2.zero, alignment = float2.zero, cohesion = float2.zero;
        int sepCount = 0, alignCount = 0, cohesionCount = 0;

        int2 cell = spatialMap.Hash(pos);
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int2 neighborCell = new(cell.x + dx, cell.y + dy);
                if (spatialMap.map.TryGetFirstValue(neighborCell, out int neighborIdx, out var it))
                {
                    do
                    {
                        if (neighborIdx != index)
                        {
                            float2 otherPos = positions[neighborIdx];
                            float dist = math.distance(pos, otherPos);

                            if (dist < settings.separationRange && dist > 0f)
                            {
                                separation += (pos - otherPos) / dist;
                                sepCount++;
                            }
                            if (dist < settings.alignmentRange)
                            {
                                alignment += velocities[neighborIdx];
                                alignCount++;
                            }
                            if (dist < settings.cohesionRange)
                            {
                                cohesion += otherPos;
                                cohesionCount++;
                            }
                        }
                    }
                    while (spatialMap.map.TryGetNextValue(out neighborIdx, ref it));
                }
            }
        }
        if (sepCount > 0) separation /= sepCount;
        if (alignCount > 0) alignment /= alignCount;
        if (cohesionCount > 0) cohesion = (cohesion / cohesionCount) - pos;

        float2 force = float2.zero;
        if (sepCount > 0) force += separation * settings.separationWeight;
        if (alignCount > 0) force += alignment * settings.alignWeight;
        if (cohesionCount > 0) force += cohesion * settings.cohesionWeight;

        forces[index] = force;

    }
}