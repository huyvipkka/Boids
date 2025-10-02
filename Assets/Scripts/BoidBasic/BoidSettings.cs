using UnityEngine;

[CreateAssetMenu(fileName = "BoidSettings", menuName = "Scriptable Objects/BoidSettings")]
public class BoidSettings : ScriptableObject
{
    [Header("Phạm vi quan sát")]
    [Range(0.1f, 10f)] public float range = 2f;

    [Header("Tốc độ")]
    [Range(1, 7)] public float minSpeed = 2f;
    [Range(2, 15)] public float maxSpeed = 6f;

    [Header("Lực tác dụng tối đa lên boid")]
    [SerializeField] public float maxSteerForce = 3f;

    [Header("Hệ số các quy tắc")]
    [Range(0, 5f)] public float separationWeight = 0.8f;
    [Range(0, 5f)] public float alignWeight = 0.4f;
    [Range(0, 5f)] public float cohesionWeight = 0.4f;
    [Range(0, 100)] public int BoundWeight = 5;
}