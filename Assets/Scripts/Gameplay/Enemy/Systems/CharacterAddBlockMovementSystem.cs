using Leopotam.EcsLite;

namespace BT
{
    public sealed class CharacterAddBlockMovementSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var entities = world
                .Filter<TakeDamageEvent>()
                .End();
            
            var damageEventPool = world.GetPool<TakeDamageEvent>();
            var blockPool = world.GetPool<BlockMovement>();
            var attackStatePool = world.GetPool<AttackState>();

            foreach (var ent in entities)
            {
                ref var damageEvent = ref damageEventPool.Get(ent);               

                if (!blockPool.Has(ent))
                {
                    ref var newBlock = ref blockPool.Add(ent);
                    newBlock.Timer = GetBlockTime(data, ref damageEvent);
                }
                else
                {
                    ref var curBlock = ref blockPool.Get(ent);
                    curBlock.Timer = GetBlockTime(data, ref damageEvent);
                }  

                if (attackStatePool.Has(ent)) attackStatePool.Del(ent);
            }
        }


        private float GetBlockTime(SharedData data, ref TakeDamageEvent evt)
        {
            return (evt.IsHammeringDamage || evt.IsPowerDamage) ? 
                data.Config.EnemyConfig.Animation.StandUpAnimationDelay : 1f;
        }        
    }
}