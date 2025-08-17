using UnityEngine;

[CreateAssetMenu(fileName = "BoidSettings", menuName = "Scriptable Objects/BoidSettings")]
public class BoidSettings : ScriptableObject
{
    [Header("Phạm vi quan sát")]
    [Range(1f, 4f)] public float separationRange = 2f;
    [Range(2f, 6f)] public float alignmentRange = 5f;
    [Range(2f, 6f)] public float cohesionRange = 5f;

    [Header("Tốc độ")]
    [Range(1, 7)] public float minSpeed = 2f;
    [Range(7, 15)] public float maxSpeed = 6f;

    [Header("Lực tác dụng tối đa lên boid")]
    [SerializeField] public float maxSteerForce = 4f;

    [Header("Hệ số các quy tắc")]
    [Range(0, 5f)] public float separationWeight = 0.05f;
    [Range(0, 5f)] public float alignWeight = 0.05f;
    [Range(0, 5f)] public float cohesionWeight = 0.005f;
    [Range(1, 100)] public int BoundWeight = 10;

}
