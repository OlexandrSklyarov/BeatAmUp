using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class CharacterTryHitSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            ClearPrevFrameDamageEvent(world);

            var attackers = world
                .Filter<TryHitEvent>()
                .End();

            var liveCharacters = world
                .Filter<Health>()
                .Inc<CharacterView>()
                .Inc<HitInteraction>()
                .Exc<TakeDamageEvent>()
                .Exc<Death>()
                .End();

            var hitEventPool = world.GetPool<TryHitEvent>();
            var hitInteractionPool = world.GetPool<HitInteraction>();
            var viewPool = world.GetPool<CharacterView>();

            foreach (var atk in attackers)
            {
                ref var hitEvent = ref hitEventPool.Get(atk);

                hitEvent.ExecuteHitTimer -= Time.deltaTime;

                //wait attack
                if (hitEvent.ExecuteHitTimer > 0f) continue;

                var (count, result) = CheckHitCount(ref hitEvent, data);

                //try hit
                for (int i = 0; i < count; i++)
                {
                    TryApplyDamage(world, result[i], liveCharacters, hitInteractionPool, viewPool, ref hitEvent);
                }

                hitEventPool.Del(atk);
            }
        }

        
        private (int count, Collider[] result) CheckHitCount(ref TryHitEvent hitEvent, SharedData data)
        {
            var service = data.CollisionService;
            
            var count = service.CheckHurtBox
            (
                hitEvent.AttackerHurtBox,
                data.Config.CharacterData.HitLayerMask
            );

            return (count, service.HitResult);
        }


        private void ClearPrevFrameDamageEvent(EcsWorld world)
        {
            var clearDamageEntities = world.Filter<TakeDamageEvent>().End();
            var damageEntityPool = world.GetPool<TakeDamageEvent>();
            
            foreach (var ent in clearDamageEntities)
            {
                damageEntityPool.Del(ent);
            }
        }


        private void TryApplyDamage(EcsWorld world, Collider col, EcsFilter liveCharacters, 
            EcsPool<HitInteraction> hitInteractionPool, EcsPool<CharacterView> viewPool, ref TryHitEvent hit)
        {
            if (!col.TryGetComponent(out HitBox receiveHitBox)) return;
            
            if (IsHitYourself(receiveHitBox, ref hit)) return;

            foreach (var ent in liveCharacters)
            {
                ref var interaction = ref hitInteractionPool.Get(ent);
                ref var view = ref viewPool.Get(ent);

                foreach (var hitBox in interaction.HitBoxes)
                {
                    if (hitBox != receiveHitBox) continue;
                    
                    IncreaseHitCounter(world, ent);
                    CreateTakeDamageEvent(world, ent, ref hit, ref view); 
                    break;
                }
            }
        }


        private bool IsHitYourself(HitBox receiveHitBox, ref TryHitEvent hit)
        {
            foreach (var hitBox in hit.IgnoredHitBoxes)
            {
                if (hitBox == receiveHitBox) return true;
            }
            
            return false;
        }


        private void IncreaseHitCounter(EcsWorld world, int damageEntity)
        {
            var counterPool = world.GetPool<HitCounter>();

            if (counterPool.Has(damageEntity))
            {
                ref var curCounter = ref counterPool.Get(damageEntity);
                curCounter.HitCount++;
                curCounter.HitResetTimer = ConstPrm.Character.HIT_COUNT_RESET_TIME; 
                return;
            }

            ref var newCounter = ref counterPool.Add(damageEntity);
            newCounter.HitCount++;
            newCounter.HitResetTimer = ConstPrm.Character.HIT_COUNT_RESET_TIME;
        }


        private void CreateTakeDamageEvent(EcsWorld world, int damageEntity, ref TryHitEvent hit, ref CharacterView view)
        {
            var damageEventPool = world.GetPool<TakeDamageEvent>();

            ref var damageEvt = ref damageEventPool.Add(damageEntity);

            damageEvt.IsHammeringDamage = hit.Type == DamageType.HAMMERING;
            damageEvt.IsPowerDamage = hit.Type == DamageType.POWERFUL;
            damageEvt.PushForce = hit.PushForce;
            damageEvt.DamageAmount = hit.Damage;
            damageEvt.HitDirection = hit.AttackerHurtBox.transform.forward;
            
            var point = hit.AttackerHurtBox.Position;
            damageEvt.HitPoint = point;
            
            damageEvt.IsTopBodyDamage = point.y >= view.ViewTransform.position.y + view.Height * 0.6f;
        }
    }
}