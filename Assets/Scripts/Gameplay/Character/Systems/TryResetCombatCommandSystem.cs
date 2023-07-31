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
                .Inc<Stun>()
                .End();

            var combatCommandPool = world.GetPool<CombatCommand>();

            foreach (var ent in entities)
            {
                ref var command = ref combatCommandPool.Get(ent);
                command.IsKick = command.IsPunch = false;                
            }
        }
    }
}