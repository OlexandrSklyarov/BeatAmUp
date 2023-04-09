using System.Linq;
using Leopotam.EcsLite;
using UnityEngine;
using Util;

namespace BT
{
    public sealed class HeroComboAttackSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var config = systems.GetShared<SharedData>().Config;

            var entities = world.Filter<HeroTag>()
                .Inc<HeroAttack>()
                .Inc<CharacterCommand>()
                .Inc<CharacterGrounded>()
                .Inc<HitInteraction>()
                .Exc<CharacterSitDown>()
                .End();

            var inputPool = world.GetPool<CharacterCommand>();
            var heroAttackPool = world.GetPool<HeroAttack>();
            var hitInteractionPool = world.GetPool<HitInteraction>();

            foreach (var e in entities)
            {
                ref var input = ref inputPool.Get(e);
                ref var attack = ref heroAttackPool.Get(e);
                ref var hitInteraction = ref hitInteractionPool.Get(e);

                ResetPreviousAttack(ref attack);
                SetComboAttack(ref input, ref attack, ref hitInteraction, world, config);
                AddActionQueue(ref input, ref attack);
                ResetComboState(ref attack); 
                ResetActionQueue(ref attack);         
            }
        }


        private void ResetPreviousAttack(ref HeroAttack attack)
        {
            attack.CurrentKick = null;
            attack.CurrentPunch = null;
        }


        private void SetComboAttack(ref CharacterCommand input, ref HeroAttack attack,  
            ref HitInteraction hitInteraction, EcsWorld world, GameConfig config)
        {
            if (attack.IsActiveAttack) return;

            if (input.IsPunch || input.IsKick) attack.IsActiveAttack = true;   

            var damage = (attack.IsActiveAttack && attack.IsNeedFinishAttack) ?
                config.HeroAttackData.MaxDamage :
                config.HeroAttackData.DefaultDamage;             

            if (input.IsPunch)
            {                
                attack.CurrentPunch = (attack.PunchQueue.Count > 0) ? 
                    attack.PunchQueue.Dequeue() : attack.PunchData[0];

                if (attack.IsNeedFinishAttack) attack.CurrentPunch = attack.PunchFinishData.RandomElement();
                
                attack.CurrentKick = null;
                attack.AttackTimer = attack.CurrentPunch.AttackTime;
                attack.ResetNextActionTimer = attack.AttackTimer * ConstPrm.Hero.ACTION_TIME_MULTIPLIER;

                CreateHitEvent(ref hitInteraction, ref attack, attack.CurrentPunch, world, damage);
            }            
            else if (input.IsKick)
            {
                attack.CurrentKick = (attack.KickQueue.Count > 0) ? 
                    attack.KickQueue.Dequeue() : attack.KickData[0];

                if (attack.IsNeedFinishAttack) attack.CurrentKick = attack.KickFinishData.RandomElement();
                                
                attack.CurrentPunch = null;
                attack.AttackTimer = attack.CurrentKick.AttackTime;
                attack.ResetNextActionTimer = attack.AttackTimer * ConstPrm.Hero.ACTION_TIME_MULTIPLIER;

                CreateHitEvent(ref hitInteraction, ref attack, attack.CurrentKick, world, damage);
            }            
        }


        private void AddActionQueue(ref CharacterCommand input, ref HeroAttack attack)
        {
            if (attack.IsNeedFinishAttack) return;

            if (input.IsPunch && attack.PunchQueue.Count < attack.PunchData.Length - 1)
            {
                attack.NextKickState = 0;

                attack.NextPunchState++;
                attack.NextPunchState %= attack.PunchData.Length;
                attack.PunchQueue.Enqueue(attack.PunchData[attack.NextPunchState]);
            }

            if (input.IsKick && attack.KickQueue.Count < attack.KickData.Length - 1)
            {
                attack.NextPunchState = 0;

                attack.NextKickState++;
                attack.NextKickState %= attack.KickData.Length;
                attack.KickQueue.Enqueue(attack.KickData[attack.NextKickState]);
            }           
        }
        

        private void CreateHitEvent(ref HitInteraction hitInteraction, ref HeroAttack attack,
            HeroAttackAnimationData attackAnimData, EcsWorld world, int damage)
        {
            var hurtBox = hitInteraction.HurtBoxes
                .FirstOrDefault(h => h.Type == attackAnimData.HitType);

            if (hurtBox == null) return;

            var hitEntity = world.NewEntity();
            var hitPool = world.GetPool<TryHitActionEvent>(); 
            ref var hit = ref hitPool.Add(hitEntity);

            hit.AttackerHurtBox = hurtBox;
            hit.IgnoredAttackerHitBoxes = hitInteraction.HitBoxes;
            hit.Damage = damage;
            hit.Timer = attackAnimData.AttackTime * attackAnimData.DamageTimeMultiplier;

            hit.Type = (attackAnimData.HitType == HitType.UP_TWO_HAND_BIG) ? DamageType.HAMMERING : 
                (attack.IsNeedFinishAttack || attack.IsCanThrowBackOpponent) ? DamageType.POWERFUL : 
                    DamageType.SIMPLE;
        }


        private void ResetComboState(ref HeroAttack attack)
        {     
            if (attack.IsActiveAttack && attack.AttackTimer <= 0f)
            {
                attack.AttackTimer = 0f;                
                attack.CurrentKick = null;
                attack.CurrentPunch = null;
                attack.IsActiveAttack = false;
                attack.IsNeedFinishAttack = false;
                attack.IsCanThrowBackOpponent = false;
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