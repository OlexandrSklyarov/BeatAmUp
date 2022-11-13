
namespace Gameplay.Character.Hero
{
    public struct HeroAttack
    {
        public PunchState CurrentPunchState;
        public KickState CurrentKickState;
        public float ComboTimer;
        public bool IsActiveCombo;
    }
}