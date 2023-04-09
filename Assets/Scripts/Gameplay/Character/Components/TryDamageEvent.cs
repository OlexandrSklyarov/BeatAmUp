using System.Collections.Generic;

namespace BT
{
    public struct TryDamageEvent
    {
        public HurtBox AttackerHurtBox;
        public IEnumerable<HitBox> IgnoredAttackerHitBoxes;
        public DamageType Type;
        public float Timer;
        public int Damage;
    }
}