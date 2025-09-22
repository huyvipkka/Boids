using System.Collections.Generic;
using System.Text;
using Unity.Profiling;
using UnityEngine;

public class PerformanceMonitor : MonoBehaviour
{
    string statsText;

    ProfilerRecorder mainThreadTimeRecorder;
    ProfilerRecorder drawCallsRecorder;
    ProfilerRecorder batchesRecorder;
    ProfilerRecorder trianglesRecorder;
    ProfilerRecorder verticesRecorder;

    float fpsTimer;
    int frames;
    float fps;

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

    void OnEnable()
    {
        mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);

        drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count", 15);
        batchesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Batches Count", 15);
        trianglesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count", 15);
        verticesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Vertices Count", 15);

    }

    void OnDisable()
    {
        mainThreadTimeRecorder.Dispose();
        drawCallsRecorder.Dispose();
        batchesRecorder.Dispose();
        trianglesRecorder.Dispose();
        verticesRecorder.Dispose();
    }

    void Update()
    {
        frames++;
        fpsTimer += Time.unscaledDeltaTime;
        if (fpsTimer >= 1f)
        {
            fps = frames / fpsTimer;
            frames = 0;
            fpsTimer = 0f;
        }
        var sb = new StringBuilder(500);

        sb.AppendLine($"Main Thread: {GetRecorderFrameAverage(mainThreadTimeRecorder) * 1e-6f:F2} ms");
        sb.AppendLine($"Draw Calls: {GetRecorderFrameAverage(drawCallsRecorder):F0}");
        sb.AppendLine($"Batches: {GetRecorderFrameAverage(batchesRecorder):F0}");
        sb.AppendLine($"Triangles: {GetRecorderFrameAverage(trianglesRecorder):F0}");
        sb.AppendLine($"Vertices: {GetRecorderFrameAverage(verticesRecorder):F0}");

        statsText = sb.ToString();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 500, 500), statsText);
    }
}
