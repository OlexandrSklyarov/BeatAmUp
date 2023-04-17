using Gameplay.FX;
using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public class CreateVfxItemSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var damageReceivers = world
                .Filter<TakeDamageEvent>()
                .End();
            
            var damageEventPool = world.GetPool<TakeDamageEvent>();
            var vfxViewPool = world.GetPool<VfxView>();
            
            foreach (var e in damageReceivers)
            {
                ref var damageEvent = ref damageEventPool.Get(e);               
                              
                CreateHitVfxEntity(world, vfxViewPool, data.VFXController, ref damageEvent); 
            }
        }
        
        private void CreateHitVfxEntity(EcsWorld world, EcsPool<VfxView> pool, 
            VisualFXController vfxController, ref TakeDamageEvent damageEvent)
        {
            var entity = world.NewEntity();
            ref var vfx = ref pool.Add(entity);
            
            var view = vfxController.PlayHitVFX(VfxType.CHARACTER_HIT, damageEvent.HitPoint);
            vfx.View = view;
            vfx.LifeTime = view.LifeTime;   
        }
    }
}