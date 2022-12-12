using UnityEngine;

namespace Gameplay.Character
{
    public struct Movement
    {
        public Transform Transform;
        public CharacterController characterController;
        public Vector3 CurrentHorizontalVelocity;
        public float VerticalVelocity;
        public float CurrentSpeed;
        public float Acceleration;
        public float MaxVelocity;

    }
}