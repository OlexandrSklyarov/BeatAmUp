using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class CharacterHitHandlerSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var config = systems.GetShared<SharedData>().Config;

            var attackingEntities = world
                .Filter<TryHitActionEvent>()
                .End();

            var liveCharacters = world
                .Filter<Health>()
                .Inc<CharacterView>()
                .Inc<HitInteraction>()
                .Exc<Death>()
                .End();

            var hitEventPool = world.GetPool<TryHitActionEvent>();
            var hitInteractionPool = world.GetPool<HitInteraction>();

            foreach (var atk in attackingEntities)
            {
                ref var hitEvent = ref hitEventPool.Get(atk);

                hitEvent.Timer -= Time.deltaTime;

                //wait attack
                if (hitEvent.Timer > 0f) continue;

                var colliders = Physics.OverlapSphere
                (
                    hitEvent.Collider.transform.position,
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
            EcsPool<HitInteraction> hitInteractionPool, ref TryHitActionEvent hitAction)
        {
            var attacker = hitAction.Attacker;

            if (!col.TryGetComponent(out IHitReceiver receiver)) return;
            if (receiver == attacker) return;            

            foreach (var entity in liveCharacters)
            {
                ref var interaction = ref hitInteractionPool.Get(entity);

                if (interaction.HitView == receiver)
                {
                    IncreaseHitCounter(world, entity);
                    CreateTakeDamageEvent(world, entity, ref hitAction); 
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


        private void CreateTakeDamageEvent(EcsWorld world, int damageEntity, ref TryHitActionEvent hitAction)
        {
            var damageEventPool = world.GetPool<TakeDamageEvent>();

            ref var damageEventComp = ref damageEventPool.Add(damageEntity);
            damageEventComp.DamageAmount = hitAction.Damage;
            damageEventComp.HitPoint = hitAction.Collider.transform.position;
            damageEventComp.IsHammeringDamage = hitAction.Type == HitType.UP_TWO_HAND_BIG;
        }
    }
}