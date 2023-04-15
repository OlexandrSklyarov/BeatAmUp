using UnityEngine;

namespace BT
{
    public struct DamageInZoneEvent
    {
        public Vector3 HitPoint;
        public Vector3 HitDirection;
        public float PushForce;
        public bool IsTopBodyDamage;
        public bool IsHammeringDamage;
        public bool IsPowerDamage;
    }
}