
namespace Gameplay.Character.Hero
{
    public struct HeroHandleAttack
    {
        public ComboState CurrentComboState;
        public float ComboTimer;
        public bool IsActiveCombo;
        public bool IsAttackProcess;
        public float Duration;
    }
}