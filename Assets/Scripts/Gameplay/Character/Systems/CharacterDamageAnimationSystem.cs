using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class CharacterDamageAnimationSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var entities = world
                .Filter<DamageInZoneEvent>()
                .Inc<CharacterView>()
                .Inc<Health>()
                .End();

            var damageInZonePool = world.GetPool<DamageInZoneEvent>();
            var ragdollEventPool = world.GetPool<ActiveRagdollEvent>();
            var viewPool = world.GetPool<CharacterView>();
            var hpPool = world.GetPool<Health>();
            var stunPool = world.GetPool<Stun>();

            foreach (var ent in entities)
            {
                ref var damage = ref damageInZonePool.Get(ent);
                ref var view = ref viewPool.Get(ent);
                ref var hp = ref hpPool.Get(ent);

                if (hp.HP <= 0f)
                {
                    AddDeathComponent(world, ent);
                    AddShakeCameraEvent(world, data);
                    DeathAnimation(ent, ragdollEventPool, ref damage, ref view);
                }
                else
                {
                    DamageAnimation(ent, ragdollEventPool, stunPool, ref damage, ref view);
                }

                damageInZonePool.Del(ent);
            }
        }

        
        private void AddDeathComponent(EcsWorld world, int damageEntity)
        {
            var pool = world.GetPool<Death>();
            ref var deathComp = ref pool.Add(damageEntity);
            deathComp.Timer = ConstPrm.Character.DEATH_TIME;            
        }


        private void AddShakeCameraEvent(EcsWorld world, SharedData data)
        {
            var eventEntity = world.NewEntity();
            var shakeEventPool = world.GetPool<ShakeCameraEvent>();
            ref var evt = ref shakeEventPool.Add(eventEntity);
            evt.Timer = data.Config.CameraConfig.ShakeDuration;
        }


        private void DeathAnimation(int entity, EcsPool<ActiveRagdollEvent> pool, 
            ref DamageInZoneEvent damage, ref CharacterView view)
        {
            if (damage.IsHammeringDamage)
            {
                PlayHammeringDamage(ref view);
            }
            else
            {
                SetDeath(ref view);
                AddRagdollEvent(entity, pool, ref view, ref damage);
            }
        }

        
        private void AddRagdollEvent(int entity, EcsPool<ActiveRagdollEvent> pool, 
            ref CharacterView view, ref DamageInZoneEvent damage)
        {
            ref var ragdollEvent = ref pool.Add(entity);
            
            ragdollEvent.PushDirection = damage.HitDirection;

            var centerBody = view.ViewTransform.position + Vector3.up * view.Height * 0.5f;
            
            ragdollEvent.NearDamagePoint = (damage.IsTopBodyDamage) ? 
                centerBody + Vector3.up * view.Height * 0.25f :
                centerBody - Vector3.up * view.Height * 0.25f;
        }


        private void SetDeath(ref CharacterView view) => view.Animator.SetBool(ConstPrm.Animation.DEATH, true);


        private void DamageAnimation(int entity, EcsPool<ActiveRagdollEvent> eventPool, EcsPool<Stun> stunPool,
            ref DamageInZoneEvent damage, ref CharacterView view)
        {
            var stunTime = ConstPrm.Character.STUN_TIME;

            if (damage.IsPowerDamage)
            {
                stunTime = ConstPrm.Character.POWER_STUN_TIME;
                AddRagdollEvent(entity, eventPool, ref view, ref damage);
            }
            else
            {
                var zoneIndex = (damage.IsTopBodyDamage) ? 0 : 1;              
                PlaySimpleDamage(ref view, zoneIndex);
            }

            AddStunComponent(stunPool, entity, stunTime);            
        }


        private void AddStunComponent(EcsPool<Stun> pool, int damageEntity, float stunTime)
        {
            if (pool.Has(damageEntity))
            {
                ref var stunComp = ref pool.Get(damageEntity);
                stunComp.Timer = stunTime;
            }
            else
            {
                ref var stunComp = ref pool.Add(damageEntity);
                stunComp.Timer = stunTime;
            }
        }


        private void PlayHammeringDamage(ref CharacterView view)
        {
            view.Animator.SetTrigger(ConstPrm.Animation.HAMMERING_DAMAGE);
        }


        private void PlaySimpleDamage(ref CharacterView view, int damageZoneIndex)
        {
            view.Animator.SetInteger(ConstPrm.Animation.DAMAGE_TYPE, damageZoneIndex);
            view.Animator.SetTrigger(ConstPrm.Animation.DAMAGE);
        }
    }
}