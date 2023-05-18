using UnityEngine;

namespace BT
{
    public struct MovementCommand
    {
        public Vector3 Direction;
        public bool IsJump;
        public bool IsRunning;
        public bool IsMoved;        
        public bool IsSitting;
    }
}