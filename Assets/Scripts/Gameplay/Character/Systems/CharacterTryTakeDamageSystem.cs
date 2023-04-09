using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class CharacterTryTakeDamageSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var config = systems.GetShared<SharedData>().Config;

            var attackingEntities = world
                .Filter<TryDamageEvent>()
                .End();

            var liveCharacters = world
                .Filter<Health>()
                .Inc<CharacterView>()
                .Inc<HitInteraction>()
                .Exc<TakeDamageEvent>()
                .Exc<Death>()
                .End();

            var hitEventPool = world.GetPool<TryDamageEvent>();
            var hitInteractionPool = world.GetPool<HitInteraction>();

            foreach (var atk in attackingEntities)
            {
                ref var hitEvent = ref hitEventPool.Get(atk);

                hitEvent.Timer -= Time.deltaTime;

                //wait attack
                if (hitEvent.Timer > 0f) continue;

                var colliders = Physics.OverlapSphere
                (
                    hitEvent.AttackerHurtBox.Collider.transform.position,
                    config.CharacterData.HitRadius,
                    config.CharacterData.HitLayerMask
                );

                //try hit
                foreach (var col in colliders)
                {
                    TryApplyDamage(world, col, liveCharacters, hitInteractionPool, ref hitEvent);
                }

                hitEventPool.Del(atk);
            }
        }


        private void TryApplyDamage(EcsWorld world, Collider col, EcsFilter liveCharacters, 
            EcsPool<HitInteraction> hitInteractionPool, ref TryDamageEvent damage)
        {
            if (!col.TryGetComponent(out HitBox receiveHitBox)) return;
            
            //if this attacker?
            foreach (var hitBox in damage.IgnoredAttackerHitBoxes)
            {
                if (hitBox == receiveHitBox) return;
            }

            foreach (var entity in liveCharacters)
            {
                ref var interaction = ref hitInteractionPool.Get(entity);

                foreach (var hitBox in interaction.HitBoxes)
                {
                    if (hitBox != receiveHitBox) continue;
                    
                    IncreaseHitCounter(world, entity);
                    CreateTakeDamageEvent(world, entity, ref damage); 
                    break;
                }
            }
        }


        private void IncreaseHitCounter(EcsWorld world, int entity)
        {
            var counterPool = world.GetPool<HitCounter>();

            if (counterPool.Has(entity))
            {
                ref var curCounter = ref counterPool.Get(entity);
                curCounter.HitCount++;
                curCounter.HitResetTimer = ConstPrm.Character.HIT_COUNT_RESET_TIME; 
                return;
            }

            ref var newCounter = ref counterPool.Add(entity);
            newCounter.HitCount++;
            newCounter.HitResetTimer = ConstPrm.Character.HIT_COUNT_RESET_TIME;
        }


        private void CreateTakeDamageEvent(EcsWorld world, int damageEntity, ref TryDamageEvent damage)
        {
            var damageEventPool = world.GetPool<TakeDamageEvent>();

            ref var damageEventComp = ref damageEventPool.Add(damageEntity);
            
            damageEventComp.DamageAmount = damage.Damage;
            damageEventComp.HitPoint = damage.AttackerHurtBox.Collider.transform.position;
            damageEventComp.IsHammeringDamage = damage.Type == DamageType.HAMMERING;
            damageEventComp.IsThrowingBody = damage.Type == DamageType.POWERFUL;

            Util.Debug.PrintColor($"TakeDamage type {damage.Type}", Color.magenta);
        }
    }
}