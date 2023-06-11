using UnityEngine.InputSystem;

namespace BT
{
    public struct CreateHeroRequest
    {
        public int SpawnIndex;
        public InputDevice Device;
        public HeroUnit Unit;
    }
}