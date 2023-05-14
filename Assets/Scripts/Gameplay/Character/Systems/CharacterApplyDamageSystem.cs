using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class CharacterApplyDamageSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var damageReceivers = world
                .Filter<TakeDamageEvent>()
                .Inc<Health>()
                .End();
            
            var damageEventPool = world.GetPool<TakeDamageEvent>();
            var hpPool = world.GetPool<Health>();
            var healthFlagPool = world.GetPool<ChangeHealthflag>();

            foreach (var ent in damageReceivers)
            {
                ref var hp = ref hpPool.Get(ent);
                ref var damageEvent = ref damageEventPool.Get(ent);

                ChangeHealth(ref hp, ref damageEvent);

                if (!healthFlagPool.Has(ent)) healthFlagPool.Add(ent);

                if (hp.CurrentHP <= 0) AddDeathComponent(world, ent);  
            }
        }
        

        private void ChangeHealth(ref Health hpComp, ref TakeDamageEvent damageEvent)
        {
            hpComp.PreviousHP = hpComp.CurrentHP;
            hpComp.CurrentHP = Mathf.Max(0, hpComp.CurrentHP - damageEvent.DamageAmount);
        }
        

        private void AddDeathComponent(EcsWorld world, int damageEntity)
        {
            var pool = world.GetPool<Death>();
            ref var deathComp = ref pool.Add(damageEntity);
            deathComp.Timer = ConstPrm.Character.DEATH_TIME;            
        }
    }
}