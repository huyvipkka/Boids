using System.Collections.Generic;
using System.Linq;

public class BoidHashManager : BoidManager
{
    private SpatialHash<BoidScript> spatialHash;
    protected override void Start()
    {
        base.Start();
        spatialHash = new(5, b => b.Position);
    }
    protected override void Update()
    {
        ResetSpatialHash();
        base.Update(); 
    }
    protected override List<BoidScript> FindNeighbors(BoidScript boid)
    {
        return spatialHash.GetNeighbors(boid.Position).ToList();
    }
    private void ResetSpatialHash()
    {
        float cellSize = settings.range;
        spatialHash.cellSize = cellSize;

        spatialHash.Clear();
        foreach (var boid in listBoid)
            spatialHash.Insert(boid);
    }
    protected override void OnDrawGizmos()
    {
        spatialHash?.DrawGizmos();
        base.OnDrawGizmos();
    }
}
