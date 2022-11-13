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
                .Inc<HeroAttack>()
                .Inc<PlayerInputData>()
                .End();

            var inputPool = world.GetPool<PlayerInputData>();
            var heroAttackPool = world.GetPool<HeroAttack>();

            foreach (var e in entities)
            {
                ref var input = ref inputPool.Get(e);
                ref var attack = ref heroAttackPool.Get(e);

                SetComboAttack(ref input, ref attack);
                ResetComboState(ref attack);          
            }
        }


        private void SetComboAttack(ref PlayerInputData input, ref HeroAttack attack)
        {
            if (attack.IsActiveCombo) return;

            if (input.IsPunch)
            {
                attack.IsActiveCombo = true;
                attack.ComboTimer = ConstPrm.Hero.DEFAULT_COMBO_TIMER;
                attack.CurrentPunchState = PunchState.PUNCH_1;
                attack.CurrentKickState = KickState.NONE;
            }            
            else if (input.IsKick)
            {
                attack.IsActiveCombo = true;
                attack.ComboTimer = ConstPrm.Hero.DEFAULT_COMBO_TIMER;
                attack.CurrentKickState = KickState.KICK_1;
                attack.CurrentPunchState = PunchState.NONE;
            }
        }
        


        private void ResetComboState(ref HeroAttack attack)
        {
            if (!attack.IsActiveCombo) return;

            if (attack.ComboTimer > 0f)
            {
                attack.ComboTimer -= Time.deltaTime;
                return;
            }

            attack.CurrentKickState = KickState.NONE;
            attack.CurrentPunchState = PunchState.NONE;
            attack.IsActiveCombo = false;
            attack.ComboTimer = 0f;
        }        
    }
}