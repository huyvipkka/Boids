using UnityEngine;

[CreateAssetMenu(fileName = "BoidSettings", menuName = "Scriptable Objects/BoidSettings")]
public class BoidSettings : ScriptableObject
{
    [Header("Phạm vi quan sát")]
    [Range(1f, 10f)] public float separationRange = 1.5f;
    [Range(1f, 6f)] public float alignmentRange = 3f;
    [Range(1f, 6f)] public float cohesionRange = 3f;

    [Header("Tốc độ")]
    [Range(1, 3)] public float minSpeed = 2f;
    [Range(4, 10)] public float maxSpeed = 6f;

    [Header("Lực tác dụng tối đa lên boid")]
    [SerializeField] public float maxSteerForce = 2f;

    [Header("Hệ số các quy tắc")]
    [Range(0, 10)] public float separationWeight = 1.5f;
    [Range(0, 10)] public float alignWeight = 1f;
    [Range(0, 10)] public float cohesionWeight = 1f;

    [Header("Hệ số đổi hướng khi gặp biên")]
    [Range(0f, 1f)] public float turnFactor = 0.5f;
}
