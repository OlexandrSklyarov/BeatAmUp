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

                ChangeHP(ref hpComp, ref damageEvent);                
                CreateHitVfxEntity(world, vfxController, damageEvent.HitPoint);
                AddStunComponent(world, e);
                AddDamageViewComponent(world, e, ref damageEvent, ref hpComp, ref view);
                TryAddDeathComponent(world, e, ref hpComp);
                
                damageEventPool.Del(e);                
            }
        }


        private void ChangeHP(ref Health hpComp, ref TakeDamageEvent damageEvent)
        {
            hpComp.PreviousHP = hpComp.HP;
            hpComp.HP = Mathf.Max(0, hpComp.HP - damageEvent.DamageAmount); 
            hpComp.IsChangeValue = true;
        }


        private void TryAddDeathComponent(EcsWorld world, int damageEntity, ref Health hpComp)
        {
            if (hpComp.HP > 0) return;

            var pool = world.GetPool<Death>();
            ref var deathComp = ref pool.Add(damageEntity);
            deathComp.Timer = ConstPrm.Character.DEATH_TIME;
        }


        private void AddDamageViewComponent(EcsWorld world, int damageEntity, 
            ref TakeDamageEvent damageEvent, ref Health hp, ref CharacterView view)
        {
            var pool = world.GetPool<DamageView>();
            ref var damageViewComp = ref pool.Add(damageEntity);
            damageViewComp.IsFinalDamage = hp.HP <= 0;
            damageViewComp.IsTopBodyDamage = damageEvent.HitPoint.y >= view.ViewTransform.position.y + view.Height * 0.6f;
            damageViewComp.IsHammeringDamage = damageEvent.IsHammeringDamage;
        }


        private void AddStunComponent(EcsWorld world, int damageEntity)
        {
            var stunPool = world.GetPool<Stun>();

            if (stunPool.Has(damageEntity))
            {
                ref var stunComp = ref stunPool.Get(damageEntity);
                stunComp.Timer = ConstPrm.Character.STUN_TIME;
            }
            else
            {
                ref var stunComp = ref stunPool.Add(damageEntity);
                stunComp.Timer = ConstPrm.Character.STUN_TIME;
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