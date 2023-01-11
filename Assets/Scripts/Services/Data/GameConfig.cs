using System;
using UnityEngine;

namespace BT
{
    [CreateAssetMenu(menuName = "SO/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [field: SerializeField] public PlayerConfig PlayerData { get; private set; }
        [field: Space(20f), SerializeField] public CharacterConfig CharacterData { get; private set; }
        [field: Space(20f), SerializeField] public CameraConfig CameraConfig { get; private set; }        
    }


    [Serializable]
    public sealed class PlayerConfig
    {
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField, Min(1f)] public float Speed { get; private set; } = 15f;
        [field: SerializeField, Min(1f)] public float ChangeVelocityTime { get; private set; } = 15f;
        [field: SerializeField, Range(0.1f, 1f)] public float ChangeVelocityTimeMultiplier { get; private set; } = 0.65f;
        [field: SerializeField, Min(1f)] public float RotateSpeed { get; private set; } = 400f;
        [field: SerializeField, Min(1f)] public float JumpForce { get; private set; } = 2f;
        [field: SerializeField, Range(0.1f, 1f)] public float AccelerationRun { get; private set; } = 1;
        [field: SerializeField, Range(0.1f, 1f)] public float AccelerationWalk { get; private set; } = 0.6f;
        [field: SerializeField, Min(0.01f)] public float AccelerationTime { get; private set; } = 50f;
        [field: SerializeField, Min(0.01f)] public float AccelerationReleaseTime { get; private set; } = 5f;
        [field: Space(10f), SerializeField] public HeroAttackAnimationData[] PunchAnimationData { get; private set; }
        [field: SerializeField] public HeroAttackAnimationData[] PunchAnimationFinishData { get; private set; }
        [field: Space(10f), SerializeField] public HeroAttackAnimationData[] KickAnimationData { get; private set; }
        [field: SerializeField] public HeroAttackAnimationData[] KickAnimationFinishData { get; private set; }
    }


    [Serializable]
    public sealed class CharacterConfig
    {
        [field: SerializeField] public LayerMask GroundLayer { get; private set; }
        [field: SerializeField, Min(0.01f)] public float CheckGroundRadius { get; private set; } = 0.4f;
        [field: SerializeField, Min(0.01f)] public float MinVerticalVelocity { get; private set; } = 2f;
    }


    [Serializable]
    public sealed class CameraConfig
    {
        [field: SerializeField] public Vector3 Offset { get; private set; }
    }
}