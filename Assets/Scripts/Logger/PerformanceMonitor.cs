using System.Collections.Generic;
using System.Text;
using Unity.Profiling;
using UnityEngine;

public class PerformanceMonitor : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Kích thước chữ hiển thị trên màn hình")]
    public int fontSize = 22;
    [Tooltip("Màu chữ hiển thị trên màn hình")]
    public Color fontColor = Color.white;
    [Tooltip("Vị trí và kích thước hộp hiển thị (x, y, width, height)")]
    public Rect displayRect = new Rect(10, 10, 600, 400);

    string statsText;

    ProfilerRecorder mainThreadTimeRecorder;
    ProfilerRecorder drawCallsRecorder;
    ProfilerRecorder batchesRecorder;
    ProfilerRecorder trianglesRecorder;
    ProfilerRecorder verticesRecorder;

    // fps
    float fpsTimer;
    int frames;
    float fps;

    static double GetRecorderFrameAverage(ProfilerRecorder recorder)
    {
        // Nếu recorder chưa khởi tạo hoặc không có mẫu, trả 0
        if (!recorder.Valid || recorder.Count == 0)
            return 0;

        var samplesCount = recorder.Count;
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
        // Lưu ý: tên mục ghi có thể khác giữa các phiên Unity / platform.
        mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);

        drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count", 15);
        batchesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Batches Count", 15);
        trianglesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count", 15);
        verticesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Vertices Count", 15);
    }

    void OnDisable()
    {
        if (mainThreadTimeRecorder.Valid) mainThreadTimeRecorder.Dispose();
        if (drawCallsRecorder.Valid) drawCallsRecorder.Dispose();
        if (batchesRecorder.Valid) batchesRecorder.Dispose();
        if (trianglesRecorder.Valid) trianglesRecorder.Dispose();
        if (verticesRecorder.Valid) verticesRecorder.Dispose();
    }

    void Update()
    {
        // Cập nhật FPS
        frames++;
        fpsTimer += Time.unscaledDeltaTime;
        if (fpsTimer >= 1f)
        {
            fps = frames / fpsTimer;
            frames = 0;
            fpsTimer = 0f;
        }

        var sb = new StringBuilder(600);

        sb.AppendLine($"FPS: {fps:F1}");
        // Main Thread time recorder thường trả giá trị tính bằng nanoseconds -> chuyển sang ms
        var mainThreadMs = GetRecorderFrameAverage(mainThreadTimeRecorder) * 1e-6f;
        sb.AppendLine($"Main Thread: {mainThreadMs:F2} ms");

        sb.AppendLine($"Draw Calls: {GetRecorderFrameAverage(drawCallsRecorder):F0}");
        sb.AppendLine($"Batches: {GetRecorderFrameAverage(batchesRecorder):F0}");
        sb.AppendLine($"Triangles: {GetRecorderFrameAverage(trianglesRecorder):F0}");
        sb.AppendLine($"Vertices: {GetRecorderFrameAverage(verticesRecorder):F0}");

        statsText = sb.ToString();
    }

    void OnGUI()
    {
        if (string.IsNullOrEmpty(statsText))
            return;

        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.Max(8, fontSize),
            wordWrap = false,
            richText = false
        };
        style.normal.textColor = fontColor;

        GUI.Label(displayRect, statsText, style);
    }
}
