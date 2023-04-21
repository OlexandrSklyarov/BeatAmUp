using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class DestroyCompletedVfxItemSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var entities = world.Filter<VfxView>().End();                                   
            var vfxPool = world.GetPool<VfxView>();

            foreach(var e in entities)
            {
                ref var vfx = ref vfxPool.Get(e);

                if (vfx.LifeTime > 0f)
                {
                    vfx.LifeTime -= Time.deltaTime;
                    continue;
                }

                vfx.View.Restore();

                vfxPool.Del(e);
            }
        }
    }
}