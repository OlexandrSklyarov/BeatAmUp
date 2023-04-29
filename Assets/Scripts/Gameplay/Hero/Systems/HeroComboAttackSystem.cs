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

            var entities = world.Filter<Hero>()
                .Inc<CharacterAttack>()
                .Inc<CharacterCommand>()
                .Inc<CharacterGrounded>()
                .Inc<HitInteraction>()
                .Exc<CharacterSitDown>()
                .End();

            var inputPool = world.GetPool<CharacterCommand>();
            var heroAttackPool = world.GetPool<CharacterAttack>();
            var hitInteractionPool = world.GetPool<HitInteraction>();

            foreach (var ent in entities)
            {
                ref var input = ref inputPool.Get(ent);
                ref var attack = ref heroAttackPool.Get(ent);
                ref var hitInteraction = ref hitInteractionPool.Get(ent);

                ResetPreviousAttack(ref attack);
                SetComboAttack(ref input, ref attack, ref hitInteraction, world, config, ent);
                AddActionQueue(ref input, ref attack, config);
                ResetComboState(ref attack); 
                ResetActionQueue(ref attack);         
            }
        }


        private void ResetPreviousAttack(ref CharacterAttack attack)
        {
            attack.CurrentKick = null;
            attack.CurrentPunch = null;
        }


        private void SetComboAttack(ref CharacterCommand input, ref CharacterAttack attack,
            ref HitInteraction hitInteraction, EcsWorld world, GameConfig config, int ent)
        {
            if (attack.IsActiveAttack) return;

            if (input.IsPunch || input.IsKick) attack.IsActiveAttack = true;
            
            if (!attack.IsActiveAttack) return;

            var isPowerfulAttack = attack.IsNeedFinishAttack || attack.IsPowerfulDamage;
            var pushForce = config.HeroAttackData.PushTargetRagdollForce;
            var damage = (isPowerfulAttack) ? config.HeroAttackData.MaxDamage : config.HeroAttackData.DefaultDamage;             
           
            attack.HitCount = (isPowerfulAttack) ? 0 : attack.HitCount;
            
            if (input.IsPunch) // punch
            {                
                attack.CurrentPunch = (attack.PunchQueue.Count > 0) ? 
                    attack.PunchQueue.Dequeue() : config.HeroAttackData.PunchAnimationData[0];

                if (isPowerfulAttack) attack.CurrentPunch = config.HeroAttackData.PunchAnimationFinishData.RandomElement();
                
                attack.CurrentKick = null;
                attack.AttackTimer = attack.CurrentPunch.AttackTime;
                attack.ResetNextActionTimer = attack.AttackTimer * ConstPrm.Hero.ACTION_TIME_MULTIPLIER;

                CreateHitEvent(ref hitInteraction, ref attack, attack.CurrentPunch, world, damage, pushForce, ent);
            }            
            else if (input.IsKick) // kick
            {
                attack.CurrentKick = (attack.KickQueue.Count > 0) ? 
                    attack.KickQueue.Dequeue() : config.HeroAttackData.KickAnimationData[0];

                if (isPowerfulAttack) attack.CurrentKick = config.HeroAttackData.KickAnimationFinishData.RandomElement();
                                
                attack.CurrentPunch = null;
                attack.AttackTimer = attack.CurrentKick.AttackTime;
                attack.ResetNextActionTimer = attack.AttackTimer * ConstPrm.Hero.ACTION_TIME_MULTIPLIER;

                CreateHitEvent(ref hitInteraction, ref attack, attack.CurrentKick, world, damage, pushForce, ent);
            }            
        }
        
        
        private void CreateHitEvent(ref HitInteraction hitInteraction, ref CharacterAttack attack,
            HeroAttackAnimationData attackAnimData, EcsWorld world, int damage, float pushForce, int ent)
        {
            var hurtBox = hitInteraction.HurtBoxes.FirstOrDefault(h => h.Type == attackAnimData.HitType);

            if (hurtBox == null) return;

            var eventPool = world.GetPool<TryHitEvent>(); 
            ref var damageEvent = ref eventPool.Add(ent);

            damageEvent.AttackerHurtBox = hurtBox;
            damageEvent.IgnoredHitBoxes = hitInteraction.HitBoxes;
            damageEvent.Damage = damage;
            damageEvent.PushForce = pushForce;
            damageEvent.ExecuteHitTimer = attackAnimData.AttackTime * attackAnimData.DamageTimeMultiplier;
            
            damageEvent.Type = (attackAnimData.HitType == HitType.TWO_HAND_POWERFUL) ? 
                DamageType.HAMMERING : (attack.IsNeedFinishAttack || attack.IsPowerfulDamage) ? 
                    DamageType.POWERFUL : DamageType.SIMPLE;
        }


        private void AddActionQueue(ref CharacterCommand input, ref CharacterAttack attack, GameConfig config)
        {
            if (attack.IsNeedFinishAttack) return;

            var punchData = config.HeroAttackData.PunchAnimationData;
            var kickData = config.HeroAttackData.KickAnimationData;

            if (input.IsPunch && attack.PunchQueue.Count < punchData.Length - 1)
            {
                attack.NextKickState = 0;

                attack.NextPunchState++;
                attack.NextPunchState %= punchData.Length;
                attack.PunchQueue.Enqueue(punchData[attack.NextPunchState]);
            }

            if (input.IsKick && attack.KickQueue.Count < kickData.Length - 1)
            {
                attack.NextPunchState = 0;

                attack.NextKickState++;
                attack.NextKickState %= kickData.Length;
                attack.KickQueue.Enqueue(kickData[attack.NextKickState]);
            }           
        }
        
        
        private void ResetComboState(ref CharacterAttack attack)
        {     
            if (attack.IsActiveAttack && attack.AttackTimer <= 0f)
            {
                attack.AttackTimer = 0f;                
                attack.CurrentKick = null;
                attack.CurrentPunch = null;
                attack.IsActiveAttack = false;
                attack.IsNeedFinishAttack = false;
                attack.IsPowerfulDamage = false;
            }
            else
            {
                attack.AttackTimer -= Time.deltaTime;
            }            
        }        


        private void ResetActionQueue(ref CharacterAttack attack)
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