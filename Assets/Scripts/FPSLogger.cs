using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

public class FPSLogger : MonoBehaviour
{
    private List<float> fpsBuffer = new();   // Lưu 10 giá trị FPS trung bình mỗi giây
    private int frameCount = 0;
    public float TimeLogFPS = 1;
    private float TimerLogFPS = 0f;
    public float TimeLogResult = 10;
    private float TimerLogResult = 0f;
    public float warmupTimer = 5f;  // 5s warmup
    public int boidStep = 10;

    private string logPath;
    private string fileName;
    private static int runIndex = 1; // số thứ tự file log

    public BoidManager manager;

    void Start()
    {
        // Tạo folder Logger trong Assets
        logPath = Application.dataPath + "/Logger";
        if (!Directory.Exists(logPath))
            Directory.CreateDirectory(logPath);

        // Đặt tên file theo scene + stt
        string sceneName = SceneManager.GetActiveScene().name;
        while (File.Exists(Path.Combine(logPath, $"{sceneName}{runIndex}.txt")))
        {
            runIndex++;
        }
        fileName = Path.Combine(logPath, $"{sceneName}{runIndex}.txt");

        Debug.Log($"Log file: {fileName}");
    }

    void Update()
    {
        if (warmupTimer > 0f)
        {
            warmupTimer -= Time.unscaledDeltaTime;
            return;
        }
        frameCount++;
        TimerLogFPS += Time.unscaledDeltaTime;
        TimerLogResult += Time.unscaledDeltaTime;

        // Mỗi 1 giây: tính FPS trung bình của 1 giây đó
        if (TimerLogFPS >= TimeLogFPS)
        {
            float fps = frameCount / TimerLogFPS;
            fpsBuffer.Add(fps);

            frameCount = 0;
            TimerLogFPS = 0f;
        }
        if (TimerLogResult >= TimeLogResult)
        {
            SaveSummary();
            fpsBuffer.Clear();
            TimerLogResult = 0f;
            manager.AddBoids(boidStep);
        }
    }

    void SaveSummary()
    {
        if (fpsBuffer.Count == 0 || manager == null) return;

        // Tính trung bình
        float avg = 0f;
        foreach (float f in fpsBuffer) avg += f;
        avg /= fpsBuffer.Count;

        float fpsmax = fpsBuffer.Max();
        float fpsmin = fpsBuffer.Min();

        // Tính độ lệch chuẩn
        float variance = 0f;
        foreach (float f in fpsBuffer) variance += (f - avg) * (f - avg);
        variance /= fpsBuffer.Count;
        float stdDev = Mathf.Sqrt(variance);

        int boidCount = manager.listBoid.Count;

        using (StreamWriter sw = new(fileName, true))
        {
            // Log FPS mảng 10 giây
            sw.WriteLine(string.Join(", ", fpsBuffer.ConvertAll(f => f.ToString("F2"))));
            sw.WriteLine($"numBoid: {boidCount}");
            sw.WriteLine($"FPS Avg: {avg:F2}");
            sw.WriteLine($"FPS Max: {fpsmax:F2}");
            sw.WriteLine($"FPS Min: {fpsmin:F2}");

            sw.WriteLine($"FPS StdDev: {stdDev:F2}");
            sw.WriteLine();
        }

        Debug.Log($"[Logger] Boids={boidCount}");
    }
}
