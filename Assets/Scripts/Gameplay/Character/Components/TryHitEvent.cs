using System.Collections.Generic;

namespace BT
{
    public struct TryHitEvent
    {
        public HurtBox AttackerHurtBox;
        public IEnumerable<HitBox> IgnoredAttackerHitBoxes;
        public DamageType Type;
        public float Timer;
        public int Damage;
        public float PushForce;
    }
}