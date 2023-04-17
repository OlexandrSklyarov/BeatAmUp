using System.Collections.Generic;

namespace BT
{
    public struct TryHitEvent
    {
        public HurtBox AttackerHurtBox;
        public IEnumerable<HitBox> IgnoredHitBoxes;
        public DamageType Type;
        public float ExecuteHitTimer;
        public int Damage;
        public float PushForce;
    }
}