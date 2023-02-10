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

                if (hitAction.Timer > 0f)
                {
                    hitAction.Timer -= Time.deltaTime;
                    continue;
                }

                var col = Physics.OverlapSphere
                (
                    hitAction.Collider.transform.position,
                    hitAction.Collider.radius,
                    config.CharacterData.HitLayerMask
                );

                foreach (var c in col)
                {
                    TryApplyDamage(c, hpViewFilter, world, ref hitAction);
                }

                hitActionPool.Del(e);
            }
        }


        private void TryApplyDamage(Collider c, EcsFilter hpViewFilter, EcsWorld world, ref HitDelayAction hitAction)
        {
            var responder = hitAction.Responder;

            var hpPool = world.GetPool<Health>();
            var viewPool = world.GetPool<CharacterView>();
            var attackPool = world.GetPool<HeroAttack>();

            if (c.TryGetComponent(out IHitReceiver receiver) && receiver != responder)
            {
                foreach (var e in hpViewFilter)
                {
                    ref var hpComp = ref hpPool.Get(e);
                    ref var viewComp = ref viewPool.Get(e);

                    if (viewComp.HitView == receiver)
                    {
                        var damageEventPool = world.GetPool<TakeDamageEvent>();
                        ref var damageEventComp = ref damageEventPool.Add(e);
                        damageEventComp.DamageAmount = hitAction.Damage;

                        foreach (var e2 in hpViewFilter)
                        {
                            ref var viewComp2 = ref viewPool.Get(e2);

                            if (viewComp.HitView == viewComp2.HitView) continue;

                            if (viewComp2.HitView == responder)
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
    }
}