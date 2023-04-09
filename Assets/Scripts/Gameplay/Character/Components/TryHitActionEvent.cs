using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public struct TryHitActionEvent
    {
        public HurtBox AttackerHurtBox;
        public IEnumerable<HitBox> IgnoredAttackerHitBoxes;
        public DamageType Type;
        public float Timer;
        public int Damage;
    }
}