using System.Linq;
using Leopotam.EcsLite;
using UnityEngine;
using Util;

namespace BT
{
    public sealed class CharacterComboAttackSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world
                .Filter<AttackData>()
                .Inc<CharacterAttack>()
                .Inc<CombatCommand>()
                .Inc<HitInteraction>()
                .End();

            var attackDataPool = world.GetPool<AttackData>();
            var inputPool = world.GetPool<CombatCommand>();
            var heroAttackPool = world.GetPool<CharacterAttack>();
            var hitInteractionPool = world.GetPool<HitInteraction>();
            var hitEventPool = world.GetPool<AttackEvent>();   

            foreach (var ent in entities)
            {
                ref var input = ref inputPool.Get(ent);
                ref var attack = ref heroAttackPool.Get(ent);
                ref var attackData = ref attackDataPool.Get(ent);
                ref var hitInteraction = ref hitInteractionPool.Get(ent);

                ResetPreviousAttack(ref attack);
                SetComboAttack(ref input, ref attack, ref hitInteraction, ref attackData, ent, hitEventPool);
                AddActionQueue(ref input, ref attack, ref attackData);
                ResetComboState(ref attack, ref input); 
                ResetActionQueue(ref attack);         
            }
        }
   

        private void ResetPreviousAttack(ref CharacterAttack attack)
        {
            attack.CurrentKick = null;
            attack.CurrentPunch = null;
        }


        private void SetComboAttack(ref CombatCommand input, ref CharacterAttack attack, ref HitInteraction hitInteraction, 
            ref AttackData attackData, int ent, EcsPool<AttackEvent> hitEventPool)
        {
            if (attack.IsActiveAttack) return;
            if (input.IsPunch || input.IsKick) attack.IsActiveAttack = true;            
            if (!attack.IsActiveAttack) return;

            var isPowerfulAttack = attack.IsNeedFinishAttack || attack.IsPowerfulDamage;
            var pushForce = attackData.Data.PushTargetRagdollForce;
            var damage = (isPowerfulAttack) ? attackData.Data.MaxDamage : attackData.Data.DefaultDamage;             
           
            attack.HitCount = (isPowerfulAttack) ? 0 : attack.HitCount;
            
            if (input.IsPunch)
            {
                PunchHandle(ref attack, ref hitInteraction, ref attackData, hitEventPool,
                    ent, isPowerfulAttack, pushForce, damage);
            }
            else if (input.IsKick)
            {
                KickHandle(ref attack, ref hitInteraction, ref attackData, hitEventPool,
                    ent, isPowerfulAttack, pushForce, damage);
            }
        }


        private void KickHandle(ref CharacterAttack attack, ref HitInteraction hitInteraction, ref AttackData attackData, 
            EcsPool<AttackEvent> hitEventPool, int ent, bool isPowerfulAttack, float pushForce, int damage)
        {
            attack.CurrentKick = (attack.KickQueue.Count > 0) ?
                attack.KickQueue.Dequeue() : attackData.Data.KickAnimationData[0];

            if (isPowerfulAttack) attack.CurrentKick = attackData.Data.KickAnimationFinishData.RandomElement();

            attack.CurrentPunch = null;
            attack.AttackTimer = attack.CurrentKick.AttackTime;
            attack.ResetNextActionTimer = attack.AttackTimer * ConstPrm.Hero.ACTION_TIME_MULTIPLIER;

            CreateHitEvent(ref hitInteraction, ref attack, hitEventPool, attack.CurrentKick, damage, pushForce, ent);
        }


        private void PunchHandle(ref CharacterAttack attack, ref HitInteraction hitInteraction, ref AttackData attackDat, 
            EcsPool<AttackEvent> hitEventPool, int ent, bool isPowerfulAttack, float pushForce, int damage)
        {
            attack.CurrentPunch = (attack.PunchQueue.Count > 0) ?
                attack.PunchQueue.Dequeue() : attackDat.Data.PunchAnimationData[0];

            if (isPowerfulAttack) attack.CurrentPunch = attackDat.Data.PunchAnimationFinishData.RandomElement();

            attack.CurrentKick = null;
            attack.AttackTimer = attack.CurrentPunch.AttackTime;
            attack.ResetNextActionTimer = attack.AttackTimer * ConstPrm.Hero.ACTION_TIME_MULTIPLIER;

            CreateHitEvent(ref hitInteraction, ref attack, hitEventPool, attack.CurrentPunch, damage, pushForce, ent);
        }


        private void CreateHitEvent(ref HitInteraction hitInteraction, ref CharacterAttack attack, EcsPool<AttackEvent> hitEventPool,
            CharacterAttackAnimationData attackAnimData, int damage, float pushForce, int ent)
        {
            var hurtBox = hitInteraction.HurtBoxes.FirstOrDefault(h => h.Type == attackAnimData.HitType);

            if (hurtBox == null) return;

            ref var damageEvent = ref hitEventPool.Add(ent);

            damageEvent.AttackerHurtBox = hurtBox;
            damageEvent.IgnoredHitBoxes = hitInteraction.HitBoxes;
            damageEvent.ExecuteHitTimer = attackAnimData.AttackTime * attackAnimData.DamageTimeMultiplier;
            damageEvent.PushForce = pushForce;
            damageEvent.Damage = damage;
            
            damageEvent.Type = (attackAnimData.HitType == HitType.TWO_HAND_POWERFUL) ? 
                DamageType.HAMMERING : (attack.IsNeedFinishAttack || attack.IsPowerfulDamage) ? 
                    DamageType.POWERFUL : DamageType.SIMPLE;
        }


        private void AddActionQueue(ref CombatCommand input, ref CharacterAttack attack, ref AttackData attackData)
        {
            if (attack.IsNeedFinishAttack) return;

            var punchData = attackData.Data.PunchAnimationData;
            var kickData = attackData.Data.KickAnimationData;

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
        
        
        private void ResetComboState(ref CharacterAttack attack, ref CombatCommand combat)
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
            else if (attack.AttackTimer > 0f)
            {
                attack.AttackTimer -= Time.deltaTime;
            }

            combat.IsKick = combat.IsPunch = false;           
        }        


        private void ResetActionQueue(ref CharacterAttack attack)
        {
            if (attack.ResetNextActionTimer > 0f)
            {
                attack.ResetNextActionTimer -= Time.deltaTime;
                return;
            }
            
            attack.KickQueue.Clear();               
            attack.PunchQueue.Clear();    
        }
    }
}