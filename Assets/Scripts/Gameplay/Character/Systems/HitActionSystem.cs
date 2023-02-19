using System;
using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class HitActionSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var config = systems.GetShared<SharedData>().Config;

            var hitFilter = world
                .Filter<HitDelayAction>()
                .End();

            var hpViewFilter = world
                .Filter<Health>()
                .Inc<CharacterView>()
                .End();

            var hitActionPool = world.GetPool<HitDelayAction>();

            foreach (var e in hitFilter)
            {
                ref var hitAction = ref hitActionPool.Get(e);

                hitAction.Timer -= Time.deltaTime;

                if (hitAction.Timer > 0f) continue;                

                var col = Physics.OverlapSphere
                (
                    hitAction.Collider.transform.position,
                    config.CharacterData.HitRadius,
                    config.CharacterData.HitLayerMask
                );

                foreach (var c in col)
                {
                    TryApplyDamage(c, hpViewFilter, world, ref hitAction);
                }

                hitActionPool.Del(e);
            }
        }


        private void TryApplyDamage(Collider c, EcsFilter hpViewFilter, EcsWorld world, 
            ref HitDelayAction hitAction)
        {
            var responder = hitAction.Responder;

            var hpPool = world.GetPool<Health>();
            var hitInteractionPool = world.GetPool<HitInteraction>();
            var attackPool = world.GetPool<HeroAttack>();

            if (c.TryGetComponent(out IHitReceiver receiver) && receiver != responder)
            {
                foreach (var e in hpViewFilter)
                {
                    ref var hpComp = ref hpPool.Get(e);
                    ref var hitIntComp = ref hitInteractionPool.Get(e);

                    if (hitIntComp.HitView == receiver)
                    {
                        CreateTakeDamageEvent(world, e, ref hitAction);                        

                        foreach (var e2 in hpViewFilter)
                        {
                            ref var hitIntComp2 = ref hitInteractionPool.Get(e2);

                            if (hitIntComp.HitView == hitIntComp2.HitView) continue;

                            if (hitIntComp2.HitView == responder)
                            {                                
                                if (attackPool.Has(e2))
                                {
                                    ref var attackComp = ref attackPool.Get(e2);
                                    attackComp.LastTargetHP = Mathf.Max(0, hpComp.HP - hitAction.Damage);
                                }

                                break;
                            }                            
                        }

                        break;
                    }                   
                }
            }
        }


        private void CreateTakeDamageEvent(EcsWorld world, int damageEntity, ref HitDelayAction hitAction)
        {
            var damageEventPool = world.GetPool<TakeDamageEvent>();
            ref var damageEventComp = ref damageEventPool.Add(damageEntity);
            damageEventComp.DamageAmount = hitAction.Damage;
            damageEventComp.HitPoint = hitAction.Collider.transform.position;
        }
    }
}