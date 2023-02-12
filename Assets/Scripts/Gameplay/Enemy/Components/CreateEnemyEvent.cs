using UnityEngine;

namespace BT
{
    public struct CreateEnemyEvent
    {
        public Vector3 CreatePosition;
        public Quaternion CreateRotation;
        public EnemyType Type;
    }
}