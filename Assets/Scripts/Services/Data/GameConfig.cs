using System;
using UnityEngine;

namespace Services.Data
{
    [CreateAssetMenu(menuName = "SO/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [field: SerializeField] public PlayerConfig PlayerData { get; private set; }
        [field: SerializeField] public CharacterConfig CharacterData { get; private set; }
        [field: Space(10f), SerializeField] public CameraConfig CameraConfig { get; private set; }
    }


    [Serializable]
    public sealed class PlayerConfig
    {
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField, Min(1f)] public float Speed { get; private set; } = 2f;
        [field: SerializeField, Min(1f)] public float RotateSpeed { get; private set; } = 360f;
        [field: SerializeField, Min(1f)] public float JumpForce { get; private set; } = 5f;
        [field: SerializeField, Min(1f)] public float MaxDrag { get; private set; } = 2f;
        [field: SerializeField, Min(0.01f)] public float MinDrag { get; private set; } = 0.25f;
        [field: SerializeField, Min(0.01f)] public float Acceleration { get; private set; } = 15f;
        [field: SerializeField, Min(0.01f)] public float RunAcceleration { get; private set; } = 8f;
    }


    [Serializable]
    public sealed class CharacterConfig
    {
        [field: SerializeField, Min(0.01f)] public float CheckGroundDistance { get; private set; } = 0.2f;
    }


    [Serializable]
    public sealed class CameraConfig
    {
        [field: SerializeField] public Vector3 Offset { get; private set; }
    }
}