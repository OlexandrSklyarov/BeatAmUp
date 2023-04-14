using Gameplay.FX;
using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class CharacterApplyDamageSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var damageReceivers = world
                .Filter<TakeDamageEvent>()
                .Inc<Health>()
                .Inc<CharacterView>()
                .End();
            
            var damageEventPool = world.GetPool<TakeDamageEvent>();
            var viewPool = world.GetPool<CharacterView>();
            var hpPool = world.GetPool<Health>();
            var damageInZonePool = world.GetPool<DamageInZoneEvent>();
            var vfxViewPool = world.GetPool<VfxView>();

            foreach (var e in damageReceivers)
            {
                ref var hp = ref hpPool.Get(e);
                ref var view = ref viewPool.Get(e);
                ref var damageEvent = ref damageEventPool.Get(e);

                ChangeHealth(ref hp, ref damageEvent);                
                CreateHitVfxEntity(world, vfxViewPool, data.VFXController, ref damageEvent);                
                AddDamageViewComponent(damageInZonePool, e, ref damageEvent, ref view);                
                
                damageEventPool.Del(e);                
            }
        }


        private void ChangeHealth(ref Health hpComp, ref TakeDamageEvent damageEvent)
        {
            hpComp.PreviousHP = hpComp.HP;
            hpComp.HP = Mathf.Max(0, hpComp.HP - damageEvent.DamageAmount); 
            hpComp.IsChangeValue = true;
        }


        private void AddDamageViewComponent(EcsPool<DamageInZoneEvent> pool, int damageEntity, 
            ref TakeDamageEvent damageEvent, ref CharacterView view)
        {
            if (pool.Has(damageEntity)) return;

            ref var damageInZoneEvent = ref pool.Add(damageEntity);

            var isTopBody = damageEvent.HitPoint.y >= view.ViewTransform.position.y + view.Height * 0.6f;
            damageInZoneEvent.IsTopBodyDamage = isTopBody;

            damageInZoneEvent.IsHammeringDamage = damageEvent.IsHammeringDamage;
            damageInZoneEvent.IsThrowingBody = damageEvent.IsPowerDamage;

            var source = damageEvent.HitPoint;
            source.y = view.ViewTransform.position.y;
            var hitDir = Vector3.Normalize(view.ViewTransform.position - source);
            damageInZoneEvent.HitDirection = hitDir;
        }


        private void CreateHitVfxEntity(EcsWorld world, EcsPool<VfxView> pool, 
            VisualFXController vfxController, ref TakeDamageEvent damageEvent)
        {
            var view = vfxController.PlayHitVFX(VfxType.CHARACTER_HIT, damageEvent.HitPoint);

            var entity = world.NewEntity();
            
            ref var vfx = ref pool.Add(entity);
            vfx.View = view;
            vfx.LifeTime = view.LifeTime;            
        }
    }
}