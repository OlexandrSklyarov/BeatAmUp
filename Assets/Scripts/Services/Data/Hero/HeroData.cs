using UnityEngine;

namespace BT
{
    [CreateAssetMenu(fileName = "HeroConfig_Name", menuName = "SO/HeroData")]
    public class HeroData : ScriptableObject
    {
        [field: SerializeField, Range(1, 100)] public int StartHP { get; private set; } = 100;
        [field: Space(20f), SerializeField, Min(1f)] public float Speed { get; private set; } = 5.5f;
        [field: SerializeField, Min(1f)] public float ChangeVelocityTime { get; private set; } = 8f;
        [field: SerializeField, Range(0.1f, 1f)] public float ChangeVelocityTimeMultiplier { get; private set; } = 0.65f;
        [field: SerializeField, Min(1f)] public float RotateSpeed { get; private set; } = 700f;
        [field: SerializeField, Min(1f)] public float JumpForce { get; private set; } = 3f;
        [field: SerializeField, Range(0.1f, 1f)] public float AccelerationRun { get; private set; } = 1;
        [field: SerializeField, Range(0.1f, 1f)] public float AccelerationWalk { get; private set; } = 0.6f;
        [field: SerializeField, Min(0.01f)] public float AccelerationTime { get; private set; } = 10f;
        [field: SerializeField, Min(0.01f)] public float AccelerationReleaseTime { get; private set; } = 1.5f;
        [field: SerializeField, Min(0.01f)] public float SlideSpeed { get; private set; } = 10f;
        [field: SerializeField, Min(0.01f)] public float SlideRotationSpeed { get; private set; } = 60f;
        [field: Space(10f), SerializeField] public CharacterAttackData Attack { get; private set; }
    }
}
