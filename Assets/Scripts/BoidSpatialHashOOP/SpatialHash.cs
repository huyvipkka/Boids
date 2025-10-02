using System;
using System.Collections.Generic;
using UnityEngine;

public class SpatialHash<T>
{
    private readonly Func<T, Vector2> getPosition;
    public float cellSize { get; set; }
    private readonly Dictionary<Vector2Int, List<T>> cells;

    public SpatialHash(float cellSize, Func<T, Vector2> getPosition)
    {
        this.cellSize = cellSize;
        this.getPosition = getPosition;
        cells = new Dictionary<Vector2Int, List<T>>();
    }

    private Vector2Int Hash(Vector2 pos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(pos.x / cellSize),
            Mathf.FloorToInt(pos.y / cellSize)
        );
    }

    public void Clear()
    {
        cells.Clear();
    }

    public void Insert(T item)
    {
        Vector2Int cell = Hash(getPosition(item));
        if (!cells.TryGetValue(cell, out var list))
        {
            list = new List<T>();
            cells[cell] = list;
        }
        list.Add(item);
    }

    public List<T> GetNeighbors(Vector2 pos)
    {
        Vector2Int center = Hash(pos);
        List<T> res = new();
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                Vector2Int neighborCell = new(center.x + dx, center.y + dy);
                if (cells.TryGetValue(neighborCell, out var list))
                {
                    foreach (var item in list)
                    {
                        Vector2 otherPos = getPosition(item);
                        float dist = Mathf.Max(0.001f, Vector2.Distance(pos, otherPos));
                        if (dist <= cellSize)
                            res.Add(item);
                    }
                }
            }
        }
        return res;
    }

    public void DrawGizmos()
    {

        Gizmos.color = Color.white;
        foreach (var k in cells) 
        {
            Vector2Int cell = k.Key;
            Vector3 worldPos = new(cell.x * cellSize, cell.y * cellSize, 0);
            Vector3 size = new(cellSize, cellSize, 0);
            Gizmos.DrawWireCube(worldPos + size * 0.5f, size);
        }
    }
}
