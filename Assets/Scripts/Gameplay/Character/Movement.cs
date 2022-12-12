using UnityEngine;

namespace Gameplay.Character
{
    public struct Movement
    {
        public Transform Transform;
        public CharacterController characterController;
        public Vector3 HorizontalVelocity;
        public float VerticalVelocity;
        public float CurrentSpeed;
        public float Acceleration;

    }
}