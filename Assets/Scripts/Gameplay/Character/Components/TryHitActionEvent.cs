using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public struct TryHitActionEvent
    {
        public HurtBox AttackerHurtBox;
        public IEnumerable<IHitReceiver> AttackerHitBoxes;
        public SphereCollider Collider;
        public DamageType Type;
        public float Timer;
        public int Damage;
    }
}