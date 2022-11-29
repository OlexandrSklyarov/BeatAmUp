using UnityEngine;

namespace Gameplay.Character
{
    public struct Movement
    {
        public Transform Transform;
        public Rigidbody Body;
        public float CurrentSpeed;
        public float Acceleration;
    }
}