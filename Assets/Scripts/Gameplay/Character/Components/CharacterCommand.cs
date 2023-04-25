using UnityEngine;

namespace BT
{
    public struct CharacterCommand
    {
        public Vector3 Direction;
        public bool IsJump;
        public bool IsRunning;
        public bool IsMoved;
        public bool IsKick;
        public bool IsPunch;
        public bool IsSitting;
    }
}