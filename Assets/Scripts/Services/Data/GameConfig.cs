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
        [field: SerializeField, Min(0.01f)] public float AccelerationTime { get; private set; } = 50f;
        [field: SerializeField, Min(0.01f)] public float AccelerationReleaseTime { get; private set; } = 5f;
    }


    [Serializable]
    public sealed class CharacterConfig
    {
        [field: SerializeField, Min(0.01f)] public LayerMask GroundLayer { get; private set; }
    }


    [Serializable]
    public sealed class CameraConfig
    {
        [field: SerializeField] public Vector3 Offset { get; private set; }
    }
}