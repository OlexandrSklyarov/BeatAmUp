
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace BT
{
    public struct HeroInputUser
    {
        public InputHandleProvider InputProvider;
        public InputDevice Device;
        public InputUser User { get; internal set; }
    }
}