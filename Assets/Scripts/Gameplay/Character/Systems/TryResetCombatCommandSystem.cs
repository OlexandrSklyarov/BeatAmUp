using Leopotam.EcsLite;

namespace BT
{
    public sealed class TryResetCombatCommandSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world
                .Filter<CombatCommand>()
                .End();

            var combatCommandPool = world.GetPool<CombatCommand>();
            var stunPool = world.GetPool<Stun>();

            foreach (var ent in entities)
            {
                if (!stunPool.Has(ent)) continue;
                
                ref var command = ref combatCommandPool.Get(ent);
                command.IsKick = command.IsPunch = false;                
            }
        }
    }
}