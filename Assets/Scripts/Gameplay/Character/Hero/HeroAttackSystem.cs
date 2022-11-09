using Leopotam.EcsLite;
using UnityEngine;

namespace Gameplay.Character.Hero
{
    public sealed class HeroAttackSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world.Filter<HeroTag>()
                .Inc<HeroHandleAttack>()
                .Inc<PlayerInputData>()
                .End();

            var inputPool = world.GetPool<PlayerInputData>();
            var heroAttackPool = world.GetPool<HeroHandleAttack>();

            foreach (var e in entities)
            {
                ref var input = ref inputPool.Get(e);
                ref var attack = ref heroAttackPool.Get(e);

                SetComboAttack(ref input, ref attack);
                ResetComboState(ref attack);
            }
        }


        private void SetComboAttack(ref PlayerInputData input, ref HeroHandleAttack attack)
        {
            if (input.IsPunch)
            {
                //if (IsLastAttack(ref attack)) return;

                attack.CurrentComboState++;
                attack.IsActiveCombo = true;
                attack.ComboTimer = ConstPrm.Hero.DEFAULT_COMBO_TIMER;
            }            
            else if (input.IsKick)
            {
                attack.CurrentComboState = ComboState.KICK_1;
            }
        }


        private bool IsLastAttack(ref HeroHandleAttack attack)
        {
            return attack.CurrentComboState == ComboState.PUNCH_4 ||
                attack.CurrentComboState == ComboState.KICK_1 || 
                attack.CurrentComboState == ComboState.KICK_2;
        }


        private void ResetComboState(ref HeroHandleAttack attack)
        {
            if (!attack.IsActiveCombo) return;

            if (attack.ComboTimer > 0f)
            {
                attack.ComboTimer -= Time.deltaTime;
                return;
            }

            attack.CurrentComboState = ComboState.NONE;
            attack.IsActiveCombo = false;
            attack.ComboTimer = ConstPrm.Hero.DEFAULT_COMBO_TIMER;
        }
    }
}