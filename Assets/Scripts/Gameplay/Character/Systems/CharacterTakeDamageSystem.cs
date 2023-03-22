using Gameplay.FX;
using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class CharacterTakeDamageSystem : IEcsRunSystem
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

            foreach (var e in damageReceivers)
            {
                ref var hp = ref hpPool.Get(e);
                ref var view = ref viewPool.Get(e);
                ref var damageEvent = ref damageEventPool.Get(e);

                ChangeHP(ref hp, ref damageEvent);                
                CreateHitVfxEntity(world, data.VFXController, damageEvent.HitPoint);                
                AddDamageViewComponent(world, e, ref damageEvent, ref view);                
                
                damageEventPool.Del(e);                
            }
        }


        private void ChangeHP(ref Health hpComp, ref TakeDamageEvent damageEvent)
        {
            hpComp.PreviousHP = hpComp.HP;
            hpComp.HP = Mathf.Max(0, hpComp.HP - damageEvent.DamageAmount); 
            hpComp.IsChangeValue = true;
        }


        private void AddDamageViewComponent(EcsWorld world, int damageEntity, 
            ref TakeDamageEvent damageEvent, ref CharacterView view)
        {
            var pool = world.GetPool<DamageViewEvent>();

            if (pool.Has(damageEntity)) return;

            ref var damageViewEvent = ref pool.Add(damageEntity);

            var isTopBody = damageEvent.HitPoint.y >= view.ViewTransform.position.y + view.Height * 0.6f;
            damageViewEvent.IsTopBodyDamage = isTopBody;

            damageViewEvent.IsHammeringDamage = damageEvent.IsHammeringDamage;
            damageViewEvent.IsThrowingBody = damageEvent.IsThrowingBody;

            var source = damageEvent.HitPoint;
            source.y = view.ViewTransform.position.y;
            var hitDir = Vector3.Normalize(view.ViewTransform.position - source);
            damageViewEvent.HitDirection = hitDir;
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