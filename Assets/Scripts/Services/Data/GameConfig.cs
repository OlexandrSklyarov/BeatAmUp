using System;
using UnityEngine;

namespace BT
{
    [CreateAssetMenu(menuName = "SO/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [field: SerializeField, Range(1, 4)] public int MaxPlayerCount { get; private set; } = 2;
        [field: Space(20f), SerializeField] public PlayerConfig PlayerData { get; private set; }
        [field: Space(20f), SerializeField] public CharacterConfig CharacterData { get; private set; }
        [field: Space(20f), SerializeField] public HeroAttackDataConfig HeroAttackData { get; private set; }
        [field: Space(20f), SerializeField] public CameraConfig CameraConfig { get; private set; }
        [field: Space(20f), SerializeField] public VfxData VfxConfig { get; private set; }
        [field: Space(20f), SerializeField] public EnemyData EnemyConfig { get; private set; }
        [field: Space(20f), SerializeField] public GameDebug GameDebugConfig { get; private set; }
    }


    [Serializable]
    public sealed class PlayerConfig
    {
        [field: SerializeField] public HeroViewProvider Prefab { get; private set; }
        [field: SerializeField, Min(1f)] public float Speed { get; private set; } = 15f;
        [field: SerializeField, Min(1f)] public float ChangeVelocityTime { get; private set; } = 15f;
        [field: SerializeField, Range(0.1f, 1f)] public float ChangeVelocityTimeMultiplier { get; private set; } = 0.65f;
        [field: SerializeField, Min(1f)] public float RotateSpeed { get; private set; } = 400f;
        [field: SerializeField, Min(1f)] public float JumpForce { get; private set; } = 2f;
        [field: SerializeField, Range(0.1f, 1f)] public float AccelerationRun { get; private set; } = 1;
        [field: SerializeField, Range(0.1f, 1f)] public float AccelerationWalk { get; private set; } = 0.6f;
        [field: SerializeField, Min(0.01f)] public float AccelerationTime { get; private set; } = 50f;
        [field: SerializeField, Min(0.01f)] public float AccelerationReleaseTime { get; private set; } = 5f;
        [field: SerializeField, Min(0.01f)] public float SlideSpeed { get; private set; } = 5f;
        [field: SerializeField, Min(0.01f)] public float SlideRotationSpeed { get; private set; } = 30f;
        [field: Space(10f), SerializeField] public int StartHP { get; private set; } = 100;
    }


    [Serializable]
    public sealed class CharacterConfig
    {
        [field: SerializeField] public LayerMask GroundLayer { get; private set; }
        [field: SerializeField, Min(0.01f)] public float CheckGroundRadius { get; private set; } = 0.4f;
        [field: SerializeField, Min(0.01f)] public float MinVerticalVelocity { get; private set; } = 2f;
        [field: SerializeField, Min(0.01f)] public float FallGravityMultiplier { get; private set; } = 2f;
        [field: Space(10f), SerializeField] public LayerMask HitLayerMask { get; private set; }
    }


    [Serializable]
    public sealed class CameraConfig
    {
        [field: SerializeField] public Vector3 Offset { get; private set; }
        [field: SerializeField, Min(0.1f)] public float CameraShakeAmplitude { get; private set; } = 1.5f;
        [field: SerializeField, Min(0.1f)] public float CameraShakeFrequency { get; private set; } = 3f;
        [field: SerializeField, Min(0.1f)] public float ShakeDuration { get; private set; } = 0.2f;
    }


    [Serializable]
    public sealed class GameDebug
    {        
        [field: SerializeField] public bool ShowDeviceInfo { get; private set; }
    }
}