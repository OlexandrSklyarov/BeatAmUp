using UnityEngine;

namespace BT
{
    [CreateAssetMenu(fileName = "CharacterAttackData_Name", menuName = "SO/CharacterAttackData")]
    public class CharacterAttackData : ScriptableObject
    {
        [field: Space(10f), SerializeField] public CharacterAttackAnimationData[] PunchAnimationData { get; private set; }
        [field: SerializeField] public CharacterAttackAnimationData[] PunchAnimationFinishData { get; private set; }
        [field: Space(10f), SerializeField] public CharacterAttackAnimationData[] KickAnimationData { get; private set; }
        [field: SerializeField] public CharacterAttackAnimationData[] KickAnimationFinishData { get; private set; }
        [field: Space(10f), SerializeField, Range(1, 100)] public int DefaultDamage { get; private set; } = 10;
        [field: Space(10f), SerializeField, Range(1, 100)] public int MaxDamage { get; private set; } = 20;
        [field: Space(10f), SerializeField, Range(5f, 10000f)] public float PushTargetRagdollForce { get; private set; } = 500f;
    }
}
