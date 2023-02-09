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

            var entities = world
                .Filter<HitDelayAction>()
                .End();

            var hitActionPool = world.GetPool<HitDelayAction>();

            foreach(var e in entities)
            {
                ref var hit = ref hitActionPool.Get(e);

                if (hit.Timer > 0f)
                {
                    hit.Timer -= Time.deltaTime;
                    continue;
                }

                var col = Physics.OverlapSphere
                (
                    hit.Collider.transform.position,
                    hit.Collider.radius,
                    config.CharacterData.HitLayerMask
                );

                foreach(var c in col)
                {
                    if (c.TryGetComponent(out IHitReceiver receiver) && receiver != hit.Responder)
                    {
                        Util.Debug.PrintColor($"hit {hit.Type} damage {hit.Damage}",Color.yellow);
                    }
                }

                hitActionPool.Del(e);
            }
        }
    }
}