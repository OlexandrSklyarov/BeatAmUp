
using UnityEngine;

namespace BT
{
    public struct TakeDamageEvent
    {
        public Vector3 HitPoint;
        public Vector3 HitDirection;
        public float PushForce;
        public int DamageAmount;
        public bool IsHammeringDamage;
        public bool IsPowerDamage;
        public bool IsTopBodyDamage;
    }
}