using UnityEngine;

namespace BT
{
    [CreateAssetMenu(menuName = "SO/HeroAttackDataConfig")]
    public class HeroAttackDataConfig : ScriptableObject
    {
        [field: Space(10f), SerializeField] public HeroAttackAnimationData[] PunchAnimationData { get; private set; }
        [field: SerializeField] public HeroAttackAnimationData[] PunchAnimationFinishData { get; private set; }
        [field: Space(10f), SerializeField] public HeroAttackAnimationData[] KickAnimationData { get; private set; }
        [field: SerializeField] public HeroAttackAnimationData[] KickAnimationFinishData { get; private set; }
        [field: Space(10f), SerializeField, Range(1, 100)] public int DefaultDamage { get; private set; } = 10;
        [field: Space(10f), SerializeField, Range(1, 100)] public int MaxDamage { get; private set; } = 20;
    }
}
