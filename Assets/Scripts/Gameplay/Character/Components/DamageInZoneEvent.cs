using UnityEngine;

namespace BT
{
    public struct DamageInZoneEvent
    {
        public Vector3 HitDirection;
        public bool IsTopBodyDamage;
        public bool IsHammeringDamage;
        public bool IsThrowingBody;
    }
}