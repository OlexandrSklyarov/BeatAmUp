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
    }
}
