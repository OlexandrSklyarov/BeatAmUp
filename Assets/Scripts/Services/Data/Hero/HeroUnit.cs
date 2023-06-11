using UnityEngine;

namespace BT
{
    [CreateAssetMenu(fileName = "Hero_name", menuName = "SO/HeroUnit")]
    public sealed class HeroUnit : ScriptableObject
    {
        [field: SerializeField] public HeroViewProvider Prefab { get; private set; }
        [field: SerializeField] public HeroData Data { get; private set; }
        [field: Space(10f), SerializeField] public CharacterAttackData Attack { get; private set; }
    }
}