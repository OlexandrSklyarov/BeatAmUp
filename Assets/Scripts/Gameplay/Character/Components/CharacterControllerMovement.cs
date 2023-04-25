using UnityEngine;

namespace BT
{
    public struct CharacterControllerMovement
    {
        public CharacterController CharacterController;
        public Vector3 HorizontalVelocity;
        public float VerticalVelocity;
        public float CurrentSpeed;
        public float PreviousSpeed;
        public float Acceleration;
        public bool IsJumpProcess;
    }
}