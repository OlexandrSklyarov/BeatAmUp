using Leopotam.EcsLite;

namespace BT
{
    public sealed class CharacterDetectDeathSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var damageReceivers = world
                .Filter<Health>()
                .Exc<Death>()
                .End();
            
            var hpPool = world.GetPool<Health>();

            foreach (var ent in damageReceivers)
            {
                ref var hp = ref hpPool.Get(ent);

                if (hp.CurrentHP <= 0) AddDeathComponent(world, ent);  
            }
        } 

        private void AddDeathComponent(EcsWorld world, int damageEntity)
        {
            var pool = world.GetPool<Death>();
            ref var deathComp = ref pool.Add(damageEntity);
            deathComp.MaxTime = deathComp.Timer = ConstPrm.Character.DEATH_TIME;            
        }
    }
}