using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Boid Settings")]
    public GameObject boidPrefab;
    public int boidCount = 4000;

    private GameObject[] boids;
    private float deltaTime;

    void Start()
    {
        boids = new GameObject[boidCount];
        for (int i = 0; i < boidCount; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-50f, 50f),
                Random.Range(-50f, 50f),
                Random.Range(-50f, 50f)
            );
            boids[i] = Instantiate(boidPrefab, pos, Quaternion.identity);
        }
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        // Simple Boid simulation (placeholder)
        Profiler.BeginSample("Boid Update");
        for (int i = 0; i < boidCount; i++)
        {
            
        }
        Profiler.EndSample();

        if (Time.frameCount % 60 == 0) // mỗi 60 frame (~1s)
        {
            float fps = 1.0f / deltaTime;
            Debug.Log($"Frame {Time.frameCount} | FPS: {fps:F1} | CPU Main: {Profiler.GetMonoUsedSizeLong()/1024f/1024f:F2} MB | Draw Calls: {UnityStats.drawCalls} | Batches: {UnityStats.batches} | Triangles: {UnityStats.triangles}");
        }
    }
}
