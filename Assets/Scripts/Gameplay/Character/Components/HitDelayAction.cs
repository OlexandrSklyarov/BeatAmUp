using UnityEngine;

namespace BT
{
    public struct HitDelayAction
    {
        public IHitReceiver Responder;
        public SphereCollider Collider;
        public HitType Type;
        public float Timer;
        public int Damage;
    }
}