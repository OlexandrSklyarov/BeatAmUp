using Leopotam.EcsLite;

namespace BT
{
    public class CharacterActiveStunSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var damageReceivers = world
                .Filter<TakeDamageEvent>()
                .Exc<Death>()
                .End();
            
            var damageEventPool = world.GetPool<TakeDamageEvent>();
            var stunPool = world.GetPool<Stun>();

            foreach (var ent in damageReceivers)
            {
                ref var damageEvent = ref damageEventPool.Get(ent);

                TryAddStun(ent, stunPool, ref damageEvent);
            }
        }
        
        
        private void TryAddStun(int damageEntity, EcsPool<Stun> pool, ref TakeDamageEvent damageEvt)
        {
            var stunTime = (damageEvt.IsHammeringDamage || damageEvt.IsPowerDamage)
                ? ConstPrm.Character.POWER_STUN_TIME
                : ConstPrm.Character.STUN_TIME;
            
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
    }
}