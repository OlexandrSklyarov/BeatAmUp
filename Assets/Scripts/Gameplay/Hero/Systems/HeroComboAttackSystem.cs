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

                SetComboAttack(ref input, ref attack, ref hitInteraction, world, config);
                AddActionQueue(ref input, ref attack);
                ResetComboState(ref attack); 
                ResetActionQueue(ref attack);         
            }
        }
        

        private void SetComboAttack(ref CharacterCommand input, ref HeroAttack attack,  
            ref HitInteraction hitInteraction, EcsWorld world, GameConfig config)
        {
            if (attack.IsActiveAttack) return;

            if (input.IsPunch || input.IsKick) attack.IsActiveAttack = true;   

            attack.CurrentDamage = config.HeroAttackData.DefaultDamage;
            
            if(attack.IsActiveAttack && 
                attack.LastTargetHP > 0 && 
                attack.LastTargetHP <= config.HeroAttackData.MaxDamage)
            {
                attack.IsNeedFinishAttack = true;
                attack.CurrentDamage = config.HeroAttackData.MaxDamage;
                Util.Debug.PrintColor("Max damage", Color.magenta);
            }

            if (input.IsPunch)
            {                
                attack.CurrentPunch = (attack.PunchQueue.Count > 0) ? 
                    attack.PunchQueue.Dequeue() : attack.PunchData[0];

                if (attack.IsNeedFinishAttack) attack.CurrentPunch = attack.PunchFinishData.RandomElement();
                
                attack.CurrentKick = null;
                attack.AttackTimer = attack.CurrentPunch.AttackTime;
                attack.ResetNextActionTimer = attack.AttackTimer * ConstPrm.Hero.ACTION_TIME_MULTIPLIER;

                CreateHitEvent(ref hitInteraction, attack.CurrentPunch, world, attack.CurrentDamage);
            }            
            else if (input.IsKick)
            {
                attack.CurrentKick = (attack.KickQueue.Count > 0) ? 
                    attack.KickQueue.Dequeue() : attack.KickData[0];

                if (attack.IsNeedFinishAttack) attack.CurrentKick = attack.KickFinishData.RandomElement();
                                
                attack.CurrentPunch = null;
                attack.AttackTimer = attack.CurrentKick.AttackTime;
                attack.ResetNextActionTimer = attack.AttackTimer * ConstPrm.Hero.ACTION_TIME_MULTIPLIER;

                CreateHitEvent(ref hitInteraction, attack.CurrentKick, world, attack.CurrentDamage);
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
        

        private void ResetComboState(ref HeroAttack attack)
        {     
            if (attack.IsActiveAttack && attack.AttackTimer <= 0f)
            {
                attack.CurrentKick = null;
                attack.CurrentPunch = null;
                attack.IsActiveAttack = false;
                attack.IsNeedFinishAttack = false;
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


        private void CreateHitEvent(ref HitInteraction hitInteraction, 
            HeroAttackAnimationData attackData, EcsWorld world, int damage)
        {
            var hitBox = hitInteraction.HitBoxes
                .FirstOrDefault(h => h.Type == attackData.HitType);

            if (hitBox == null) return;

            var hitEntity = world.NewEntity();
            var hitPool = world.GetPool<HitDelayAction>(); 
            ref var hit = ref hitPool.Add(hitEntity);

            hit.Responder = hitInteraction.HitView;
            hit.Collider = hitBox.Collider;
            hit.Type = hitBox.Type;
            hit.Damage = damage;
            hit.Timer = attackData.AttackTime * attackData.DamageTimeMultiplier;
        }
    }
}