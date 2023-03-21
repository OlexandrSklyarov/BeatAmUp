using UnityEngine;

namespace BT
{
    public struct TryHitActionEvent
    {
        public IHitReceiver Attacker;
        public SphereCollider Collider;
        public HitType Type;
        public float Timer;
        public int Damage;
    }
}