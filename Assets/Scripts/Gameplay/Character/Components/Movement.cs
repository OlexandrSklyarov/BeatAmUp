using UnityEngine;

namespace BT
{
    public struct Movement
    {
        public Transform Transform;
        public CharacterController characterController;
        public Vector3 HorizontalVelocity;
        public float VerticalVelocity;
        public float CurrentSpeed;
        public float PreviousSpeed;
        public float Acceleration;

    }
}