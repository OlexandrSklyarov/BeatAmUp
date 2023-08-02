using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class CharacterHitSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            ClearPrevFrameDamageEvent(world);

            var attackers = world
                .Filter<AttackEvent>()
                .Inc<CharacterAttack>()
                .End();

            var liveCharacters = world
                .Filter<Health>()
                .Inc<CharacterView>()
                .Inc<HitInteraction>()
                .Exc<TakeDamageEvent>()
                .Exc<Death>()
                .End();

            var hitEventPool = world.GetPool<AttackEvent>();
            var hitInteractionPool = world.GetPool<HitInteraction>();
            var viewPool = world.GetPool<CharacterView>();
            var attackPool = world.GetPool<CharacterAttack>();            

            foreach (var atk in attackers)
            {
                ref var hitEvent = ref hitEventPool.Get(atk);
                ref var attack = ref attackPool.Get(atk);

                if (!IsHitTimeEnd(ref hitEvent)) continue;

                var (count, result) = CheckHitCount(ref hitEvent, data);

                //try hit
                for (int i = 0; i < count; i++)
                {
                    TryApplyDamage(world, result[i], liveCharacters, hitInteractionPool, viewPool, 
                        ref hitEvent, ref attack);
                }

                hitEventPool.Del(atk);
            }
        }


        private bool IsHitTimeEnd(ref AttackEvent hitEvent)
        {
            hitEvent.ExecuteHitTimer -= Time.deltaTime;
            return hitEvent.ExecuteHitTimer <= 0f;
        }   

        
        private (int count, Collider[] result) CheckHitCount(ref AttackEvent hitEvent, SharedData data)
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
            EcsPool<HitInteraction> hitInteractionPool, EcsPool<CharacterView> viewPool, 
            ref AttackEvent hit, ref CharacterAttack attack)
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
                    
                    IncreaseHitCounter(ref attack);
                    CreateTakeDamageEvent(world, ent, ref hit, ref view); 
                    break;
                }
            }
        }


        private bool IsHitYourself(HitBox receiveHitBox, ref AttackEvent hit)
        {
            foreach (var hitBox in hit.IgnoredHitBoxes)
            {
                if (hitBox == receiveHitBox) return true;
            }
            
            return false;
        }


        private void IncreaseHitCounter(ref CharacterAttack attack)
        {
            attack.HitCount++;
            attack.HitResetTimer = ConstPrm.Character.HIT_COUNT_RESET_TIME;
        }


        private void CreateTakeDamageEvent(EcsWorld world, int damageEntity, ref AttackEvent hit, ref CharacterView view)
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