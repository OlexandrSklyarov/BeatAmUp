using UnityEngine;

namespace Gameplay.Character.Hero
{
    public struct PlayerInputData
    {
        public Vector3 Direction;
        public bool IsJump;
        public bool IsRunning;
        public bool IsMoved;
        public bool IsKick;
        public bool IsPunch;
    }
}