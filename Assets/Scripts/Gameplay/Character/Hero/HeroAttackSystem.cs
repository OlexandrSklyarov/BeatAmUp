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
                CompletedAttackTime(ref attack);            
            }
        }


        private void SetComboAttack(ref PlayerInputData input, ref HeroHandleAttack attack)
        {
            if (attack.IsAttackProcess) return;

            if (input.IsPunch)
            {
                attack.CurrentComboState++;
                attack.IsActiveCombo = true;
                attack.IsAttackProcess = true;
                attack.Duration = ConstPrm.Hero.ATTACK_TIMER;
                attack.ComboTimer = ConstPrm.Hero.DEFAULT_COMBO_TIMER;
            }            
            else if (input.IsKick)
            {
                attack.CurrentComboState = ComboState.KICK_1;
                attack.IsActiveCombo = true;
                attack.IsAttackProcess = true;
                attack.Duration = ConstPrm.Hero.ATTACK_TIMER;
            }
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


        private void CompletedAttackTime(ref HeroHandleAttack attack)
        {
            if (!attack.IsAttackProcess) return;

            if (attack.Duration > 0f)
            {
                attack.Duration -= Time.deltaTime;
                return;
            }

            attack.IsAttackProcess = false;
        }
    }
}