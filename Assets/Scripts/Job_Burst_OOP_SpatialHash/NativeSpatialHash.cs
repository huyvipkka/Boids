using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public struct NativeSpatialHash : System.IDisposable
{
    public NativeParallelMultiHashMap<int2, int> map;
    public float cellSize;
    public NativeSpatialHash(int capacity, float cellSize, Allocator allocator)
    {
        this.cellSize = cellSize;
        map = new(capacity, allocator);
    }

    public int2 Hash(float2 pos)
    {
        return new int2(
            (int)math.floor(pos.x / cellSize),
            (int)math.floor(pos.y / cellSize)
        );
    }

    public void Insert(float2 pos, int index)
    {
        int2 cell = Hash(pos);
        map.Add(cell, index);
    }
    public void Dispose()
    {
        if (map.IsCreated) map.Dispose();
    }
    public void Clear()
    {
        map.Clear();
    }

    public void DrawGizmos()
    {
        Gizmos.color = Color.green;

        var keys = map.GetKeyArray(Allocator.Temp);
        var seen = new NativeHashSet<int2>(keys.Length, Allocator.Temp);

        foreach (int2 cell in keys)
        {
            if (!seen.Add(cell)) continue; // tránh vẽ trùng

            Vector3 worldPos = new Vector3(cell.x * cellSize, cell.y * cellSize, 0);
            Vector3 size = new Vector3(cellSize, cellSize, 0);

            Gizmos.DrawWireCube(worldPos + size * 0.5f, size);
        }

        keys.Dispose();
        seen.Dispose();
    }
}