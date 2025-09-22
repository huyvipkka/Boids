using UnityEngine;
using Unity.Profiling;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class PerfLoggerCSV : MonoBehaviour
{
    [SerializeField] private BoidManager boidManager;
    [SerializeField] private int flushTime = 10;

    private StreamWriter writer;
    private float flushTimer = 0f;

    private ProfilerRecorder cpuMain, drawCalls, batches, tris, verts;

    // FPS tracking
    private int frames = 0;
    private float fpsTimer = 0f;
    private float fpsCurrent = 0f;
    private List<float> fpsList = new();

    void OnEnable()
    {
        string folderPath = Path.Combine(Application.dataPath, "Logger");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string sceneName = SceneManager.GetActiveScene().name;
        int index = 1;
        string filePath;
        do
        {
            filePath = Path.Combine(folderPath, $"{sceneName}_{index}.csv");
            index++;
        } while (File.Exists(filePath));

        writer = new StreamWriter(filePath, false);

        // --- BoidCount lên đầu ---
        writer.WriteLine("BoidCount,Frame,FPS,FPS_Avg,FPS_Min,FPS_Max,CPU_Main(ms),DrawCalls,Batches,Triangles,Vertices");

        cpuMain   = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 300);
        drawCalls = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count", 300);
        batches   = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Batches Count", 300);
        tris      = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count", 300);
        verts     = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Vertices Count", 300);
    }

    void Update()
    {
        // --- FPS bằng đếm frame ---
        frames++;
        fpsTimer += Time.unscaledDeltaTime;
        if (fpsTimer >= 1f)
        {
            fpsCurrent = frames / fpsTimer;
            fpsList.Add(fpsCurrent);
            frames = 0;
            fpsTimer = 0f;
        }

        flushTimer += Time.deltaTime;
        if (flushTimer >= flushTime)
        {
            // Tính Avg, Min, Max của FPS
            float fpsAvg = fpsList.Count > 0 ? fpsList.Average() : 0f;
            float fpsMin = fpsList.Count > 0 ? fpsList.Min() : 0f;
            float fpsMax = fpsList.Count > 0 ? fpsList.Max() : 0f;
            int boidCount = boidManager.listBoid.Count;

            double cpuMainMs = GetRecorderFrameAverage(cpuMain) * 1e-6f;
            double dcAvg     = GetRecorderFrameAverage(drawCalls);
            double batchAvg  = GetRecorderFrameAverage(batches);
            double trisAvg   = GetRecorderFrameAverage(tris);
            double vertsAvg  = GetRecorderFrameAverage(verts);

            // --- BoidCount lên đầu ---
            writer.WriteLine($"{boidCount},{Time.frameCount},{fpsCurrent},{fpsAvg},{fpsMin},{fpsMax}," +
                             $"{cpuMainMs:F2},{dcAvg:F0},{batchAvg:F0},{trisAvg:F0},{vertsAvg:F0}");
            writer.Flush();
            fpsList.Clear();
            flushTimer = 0f;
        }
    }

    void OnDisable()
    {
        writer?.Close();
        cpuMain.Dispose();
        drawCalls.Dispose();
        batches.Dispose();
        tris.Dispose();
        verts.Dispose();
    }

    static double GetRecorderFrameAverage(ProfilerRecorder recorder)
    {
        var samplesCount = recorder.Capacity;
        if (samplesCount == 0)
            return 0;
        double r = 0;
        unsafe
        {
            var samples = stackalloc ProfilerRecorderSample[samplesCount];
            recorder.CopyTo(samples, samplesCount);
            for (var i = 0; i < samplesCount; ++i)
                r += samples[i].Value;
            r /= samplesCount;
        }
        return r;
    }
}
