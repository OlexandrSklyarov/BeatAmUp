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
                .Inc<CharacterCommand>()
                .End();

            var inputPool = world.GetPool<CharacterCommand>();
            var heroAttackPool = world.GetPool<HeroAttack>();

            foreach (var e in entities)
            {
                ref var input = ref inputPool.Get(e);
                ref var attack = ref heroAttackPool.Get(e);

                SetComboAttack(ref input, ref attack);
                AddActionQueue(ref input, ref attack);
                ResetComboState(ref attack); 
                ResetActionQueue(ref attack);         
            }
        }


        private void AddActionQueue(ref CharacterCommand input, ref HeroAttack attack)
        {
            if (input.IsKick && attack.KickQueue.Count < ConstPrm.Hero.MAX_KICK_ACTIONS)
            {
                attack.NextKickState++;
                attack.NextKickState %= ConstPrm.Hero.MAX_KICK_ACTIONS;
                attack.NextKickState = Mathf.Max(attack.NextKickState, 1);
                attack.KickQueue.Enqueue((KickState)attack.NextKickState);
            }

            if (input.IsPunch && attack.PunchQueue.Count < ConstPrm.Hero.MAX_PUNCH_ACTIONS)
            {
                attack.NextPunchState++;
                attack.NextPunchState %= ConstPrm.Hero.MAX_PUNCH_ACTIONS;
                attack.NextPunchState = Mathf.Max(attack.NextPunchState, 1);
                attack.PunchQueue.Enqueue((PunchState)attack.NextPunchState);
            }
        }


        private void SetComboAttack(ref CharacterCommand input, ref HeroAttack attack)
        {
            if (attack.IsActiveAttack) return;

            if (input.IsPunch || input.IsKick)
            {
                attack.IsActiveAttack = true;                
            }

            if (input.IsPunch)
            {
                attack.CurrentPunchState = (attack.PunchQueue.Count > 0) ? 
                    attack.PunchQueue.Dequeue() : PunchState.PUNCH_1;
                
                attack.CurrentKickState = KickState.NONE;
                attack.AttackTimer = ConstPrm.Hero.PUNCH_COMBO_TIME;
                attack.ResetNextActionTimer = ConstPrm.Hero.PUNCH_COMBO_TIME * ConstPrm.Hero.ACTION_TIME_MULTIPLIER;
            }            
            else if (input.IsKick)
            {
                attack.CurrentKickState = (attack.KickQueue.Count > 0) ? 
                    attack.KickQueue.Dequeue() : KickState.KICK_1;
                
                attack.CurrentPunchState = PunchState.NONE;
                attack.AttackTimer = ConstPrm.Hero.KICK_COMBO_TIME;
                attack.ResetNextActionTimer = ConstPrm.Hero.KICK_COMBO_TIME * ConstPrm.Hero.ACTION_TIME_MULTIPLIER;
            }
        }
        


        private void ResetComboState(ref HeroAttack attack)
        {     
            if (attack.IsActiveAttack && attack.AttackTimer <= 0f)
            {
                attack.CurrentKickState = KickState.NONE;
                attack.CurrentPunchState = PunchState.NONE;
                attack.IsActiveAttack = false;
                attack.AttackTimer = 0f;                
            }
            else
            {
                attack.AttackTimer -= Time.deltaTime;
            }            
        }        


        private void ResetActionQueue(ref HeroAttack attack)
        {
            if (attack.ResetNextActionTimer <= 0f)
            {
                attack.KickQueue.Clear();               
                attack.PunchQueue.Clear();               
            }
            else
            {
                attack.ResetNextActionTimer -= Time.deltaTime;
            }
        }
    }
}