using System;
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

            var entities = world.Filter<HeroTag>()
                .Inc<HeroAttack>()
                .Inc<CharacterCommand>()
                .Inc<CharacterGrounded>()
                .Exc<CharacterSitDown>()
                .End();

            var inputPool = world.GetPool<CharacterCommand>();
            var heroAttackPool = world.GetPool<HeroAttack>();

            foreach (var e in entities)
            {
                ref var input = ref inputPool.Get(e);
                ref var attack = ref heroAttackPool.Get(e);

                SetComboAttack(ref input, ref attack, world);
                AddActionQueue(ref input, ref attack);
                ResetComboState(ref attack); 
                ResetActionQueue(ref attack);         
            }
        }
        

        private void SetComboAttack(ref CharacterCommand input, ref HeroAttack attack,  EcsWorld world)
        {
            if (attack.IsActiveAttack) return;

            if (input.IsPunch || input.IsKick) attack.IsActiveAttack = true;   

            if (input.IsPunch)
            {                
                attack.CurrentPunch = (attack.PunchQueue.Count > 0) ? 
                    attack.PunchQueue.Dequeue() : attack.PunchData[0];

                if (attack.IsNeedFinishAttack) attack.CurrentPunch = attack.PunchFinishData.RandomElement();
                
                attack.CurrentKick = null;
                attack.AttackTimer = attack.CurrentPunch.AttackTime;
                attack.ResetNextActionTimer = attack.AttackTimer * ConstPrm.Hero.ACTION_TIME_MULTIPLIER;

                CreateHitEvent(attack.HitBoxes, attack.HitOwner, attack.CurrentPunch, world);
            }            
            else if (input.IsKick)
            {
                attack.CurrentKick = (attack.KickQueue.Count > 0) ? 
                    attack.KickQueue.Dequeue() : attack.KickData[0];

                if (attack.IsNeedFinishAttack) attack.CurrentKick = attack.KickFinishData.RandomElement();
                                
                attack.CurrentPunch = null;
                attack.AttackTimer = attack.CurrentKick.AttackTime;
                attack.ResetNextActionTimer = attack.AttackTimer * ConstPrm.Hero.ACTION_TIME_MULTIPLIER;

                CreateHitEvent(attack.HitBoxes, attack.HitOwner, attack.CurrentKick, world);
            }
        }


        private void AddActionQueue(ref CharacterCommand input, ref HeroAttack attack)
        {
            if (attack.IsNeedFinishAttack) return;

            if (input.IsPunch && attack.PunchQueue.Count < attack.PunchData.Length - 1)
            {
                attack.NextPunchState++;
                attack.NextPunchState %= attack.PunchData.Length;
                attack.PunchQueue.Enqueue(attack.PunchData[attack.NextPunchState]);
            }

            if (input.IsKick && attack.KickQueue.Count < attack.KickData.Length - 1)
            {
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


        private void CreateHitEvent(HitBox[] hitBoxes, IHitReceiver responder, HeroAttackAnimationData attackData, EcsWorld world)
        {
            var hitBox = hitBoxes.FirstOrDefault(h => h.Type == attackData.HitType);

            if (hitBox == null) return;

            var hitEntity = world.NewEntity();
            var hitPool = world.GetPool<HitDelayAction>(); 
            ref var hit = ref hitPool.Add(hitEntity);

            hit.Responder = responder;
            hit.Collider = hitBox.Collider;
            hit.Type = hitBox.Type;
            hit.Damage = 1;
            hit.Timer = attackData.AttackTime * 0.65f;
        }
    }
}