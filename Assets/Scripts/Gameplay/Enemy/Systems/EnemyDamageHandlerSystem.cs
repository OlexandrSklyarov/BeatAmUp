using Leopotam.EcsLite;
using UnityEngine;
using Util;

namespace BT
{
    public sealed class EnemyDamageHandlerSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var entities = world
                .Filter<DamageInZoneEvent>()
                .Inc<CharacterView>()
                .Inc<Health>()
                .Inc<CharacterPhysicsBody>()
                .End();

            var damageViewPool = world.GetPool<DamageInZoneEvent>();
            var viewPool = world.GetPool<CharacterView>();
            var hpPool = world.GetPool<Health>();
            var bodyPool = world.GetPool<CharacterPhysicsBody>();

            foreach (var e in entities)
            {
                ref var damageView = ref damageViewPool.Get(e);
                ref var view = ref viewPool.Get(e);
                ref var hp = ref hpPool.Get(e);
                ref var body = ref bodyPool.Get(e);

                if (hp.HP <= 0f)
                {
                    AddDeathComponent(world, e);
                    AddShakeCameraEvent(world, data);
                    DeathProcess(ref damageView, ref view);
                }
                else
                {
                    DamageProcess(world, e, ref damageView, ref view);
                }

                damageViewPool.Del(e);
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


        private void DeathProcess(ref DamageInZoneEvent damageInZone, ref CharacterView view)
        {
            if (damageInZone.IsHammeringDamage)
            {
                PlayHammeringDamage(ref view);
            }
            else
            {
                SetDeath(ref view);
                PlayThrowBody(ref view, ref damageInZone);
            }
        }


        private void SetDeath(ref CharacterView view) => view.Animator.SetBool(ConstPrm.Animation.DEATH, true);


        private void DamageProcess(EcsWorld world, int entity, ref DamageInZoneEvent damageInZone, ref CharacterView view)
        {
            var stunTime = ConstPrm.Character.STUN_TIME;

            if (damageInZone.IsThrowingBody)
            {
                stunTime = PlayThrowBody(ref view, ref damageInZone);
            }
            else
            {
                SetDamageTypeAnimation(ref damageInZone, ref view);                
                PlayDamage(ref view);
            }

            AddStunComponent(world, entity, stunTime);            
        }


        private void AddStunComponent(EcsWorld world, int damageEntity, float stunTime)
        {
            var stunPool = world.GetPool<Stun>();

            if (stunPool.Has(damageEntity))
            {
                ref var stunComp = ref stunPool.Get(damageEntity);
                stunComp.Timer = stunTime;
            }
            else
            {
                ref var stunComp = ref stunPool.Add(damageEntity);
                stunComp.Timer = stunTime;
            }
        }


        private void SetDamageTypeAnimation(ref DamageInZoneEvent damageInZone, ref CharacterView view)
        {
            var damageZone = (damageInZone.IsTopBodyDamage) ? 0 : 1;
            view.Animator.SetInteger(ConstPrm.Animation.DAMAGE_TYPE, damageZone);
        }


        private void PlayHammeringDamage(ref CharacterView view) => 
            view.Animator.SetTrigger(ConstPrm.Animation.HAMMERING_DAMAGE);
        
        
        private void PlayDamage(ref CharacterView view) => 
            view.Animator.SetTrigger(ConstPrm.Animation.DAMAGE);
        
        
        private float PlayThrowBody(ref CharacterView view, ref DamageInZoneEvent damageInZone) 
        {
            view.ViewTransform.rotation = Quaternion.RotateTowards
            (
                view.ViewTransform.rotation,
                Util.Vector3Math.DirToQuaternion(damageInZone.HitDirection * -1f),
                360f
            );

            var state = view.Animator.GetCurrentAnimatorStateInfo(0);
            var isThrowing = state.IsName(ConstPrm.Animation.THROW_BODY);

            if (!isThrowing) view.Animator.SetTrigger(ConstPrm.Animation.THROW_BODY);

            var length = 1.5f;//view.Animator.GetLengthState(ConstPrm.Animation.THROW_BODY);
            Util.Debug.Print($"PlayThrowBody len state {length}");
            return length;
        }
    }
}