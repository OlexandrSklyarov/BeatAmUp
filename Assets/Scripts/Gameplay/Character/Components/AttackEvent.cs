using System.Collections.Generic;

namespace BT
{
    public struct AttackEvent
    {
        public IEnumerable<HitBox> IgnoredHitBoxes;
        public HurtBox AttackerHurtBox;
        public DamageType Type;
        public float ExecuteHitTimer;
        public float PushForce;
        public int Damage;
    }
}