using System;
using Gameplay.FX;
using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class TakeDamageSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var config = systems.GetShared<SharedData>().Config;
            var vfxController = systems.GetShared<SharedData>().VFXController;

            var filter = world
                .Filter<TakeDamageEvent>()
                .Inc<Health>()
                .Inc<CharacterView>()
                .End();
            
            var damageEventPool = world.GetPool<TakeDamageEvent>();
            var viewPool = world.GetPool<CharacterView>();
            var hpPool = world.GetPool<Health>();

            foreach (var e in filter)
            {
                ref var hpComp = ref hpPool.Get(e);
                ref var view = ref viewPool.Get(e);
                ref var damageEvent = ref damageEventPool.Get(e);

                var prev = hpComp.HP;
                hpComp.HP = Mathf.Max(0, hpComp.HP - damageEvent.DamageAmount); 

                view.HitView.Hit();
                Util.Debug.PrintColor($"Add hit {view.HitView} damage {damageEvent.DamageAmount} hp {prev}/{hpComp.HP}", Color.yellow);                 
                
                CreateHitVfxEntity(world, vfxController, damageEvent.HitPoint);
                
                damageEventPool.Del(e);                
            }
        }


        private void CreateHitVfxEntity(EcsWorld world, VisualFXController vfxController, Vector3 hitPoint)
        {
            var view = vfxController.PlayHitVFX(VfxType.CHARACTER_HIT, hitPoint);

            var entity = world.NewEntity();
            var vfxPool = world.GetPool<VfxView>();
            ref var vfx = ref vfxPool.Add(entity);
            vfx.View = view;
            vfx.LifeTime = view.LifeTime;            
        }
    }
}