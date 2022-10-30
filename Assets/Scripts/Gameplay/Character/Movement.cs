using UnityEngine;

namespace Gameplay.Character
{
    public struct Movement
    {
        public Transform Transform;
        public Transform ViewTransform;
        public Rigidbody Body;
        public float CurrentSpeed;
        public float Acceleration;
        public bool IsGround;
    }
}