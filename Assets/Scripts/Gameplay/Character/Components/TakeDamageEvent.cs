
using UnityEngine;

namespace BT
{
    public struct TakeDamageEvent
    {
        public Vector3 HitPoint;
        public int DamageAmount;
        public bool IsHammeringDamage;
        public bool IsPowerDamage;
    }
}