using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class CharacterTryHitSystem : IEcsInitSystem, IEcsRunSystem
    {
        private Collider[] _result;
        
        public void Init(IEcsSystems systems)
        {
            _result = new Collider[10];
        }
        

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var config = systems.GetShared<SharedData>().Config;

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

            foreach (var atk in attackers)
            {
                ref var hitEvent = ref hitEventPool.Get(atk);

                hitEvent.ExecuteHitTimer -= Time.deltaTime;

                //wait attack
                if (hitEvent.ExecuteHitTimer > 0f) continue;

                var count = CheckHitCount(ref hitEvent, config, _result);

                //try hit
                for (int i = 0; i < count; i++)
                {
                    TryApplyDamage(world, _result[i], liveCharacters, hitInteractionPool, ref hitEvent);
                }

                hitEventPool.Del(atk);
            }
        }
        

        private int CheckHitCount(ref TryHitEvent hitEvent, GameConfig config, Collider[] result)
        {
            return Physics.OverlapBoxNonAlloc
            (
                hitEvent.AttackerHurtBox.Position,
                hitEvent.AttackerHurtBox.HalfExtend,
                result,
                Quaternion.identity,
                config.CharacterData.HitLayerMask
            );
        }


        private void TryApplyDamage(EcsWorld world, Collider col, EcsFilter liveCharacters, 
            EcsPool<HitInteraction> hitInteractionPool, ref TryHitEvent hit)
        {
            if (!col.TryGetComponent(out HitBox receiveHitBox)) return;
            
            if (IsHitYourself(receiveHitBox, ref hit)) return;

            foreach (var ent in liveCharacters)
            {
                ref var interaction = ref hitInteractionPool.Get(ent);

                foreach (var hitBox in interaction.HitBoxes)
                {
                    if (hitBox != receiveHitBox) continue;
                    
                    IncreaseHitCounter(world, ent);
                    CreateTakeDamageEvent(world, ent, ref hit); 
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


        private void CreateTakeDamageEvent(EcsWorld world, int damageEntity, ref TryHitEvent hit)
        {
            var damageEventPool = world.GetPool<TakeDamageEvent>();

            ref var damageEvent = ref damageEventPool.Add(damageEntity);

            damageEvent.PushForce = hit.PushForce;
            damageEvent.DamageAmount = hit.Damage;
            damageEvent.HitDirection = hit.AttackerHurtBox.transform.forward;
            damageEvent.HitPoint = hit.AttackerHurtBox.Position;
            damageEvent.IsHammeringDamage = hit.Type == DamageType.HAMMERING;
            damageEvent.IsPowerDamage = hit.Type == DamageType.POWERFUL;

            Util.Debug.PrintColor($"TakeDamage type {hit.Type}", Color.magenta);
        }
    }
}