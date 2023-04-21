using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public static class GameplayExtensions
    {
        public static void CreateVfxEntity(EcsWorld world, EcsPool<VfxView> pool, 
            SharedData data, Vector3 position, VfxType type)
        {
            var entity = world.NewEntity();
            ref var vfx = ref pool.Add(entity);
            
            var view = data.VFXController.PlayHitVFX(type, position);
            vfx.View = view;
            vfx.LifeTime = view.LifeTime;   
        }
    }
}