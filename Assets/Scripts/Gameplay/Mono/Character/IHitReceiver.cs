using UnityEngine;

namespace BT
{
    public interface IHitReceiver
    {
        void AddForceDamage(Vector3 force);
    }
}