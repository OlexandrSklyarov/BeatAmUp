using Leopotam.EcsLite;

namespace BT
{
    public sealed class StopMovementWhenAttackingSystem : IEcsRunSystem
    {
        private const float MIN_VELOCITY_MULTIPLIER = 0.01f;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world
                .Filter<HeroAttack>()
                .Inc<Movement>()  
                .Inc<CharacterGrounded>()  
                .End();

            var attackPool = world.GetPool<HeroAttack>();
            var movementPool = world.GetPool<Movement>();

            foreach(var e in entities)
            {
                ref var attack = ref attackPool.Get(e);
                ref var movement = ref movementPool.Get(e);
                
                if (attack.IsActiveAttack) 
                {                    
                    movement.HorizontalVelocity.x *= MIN_VELOCITY_MULTIPLIER;
                    movement.HorizontalVelocity.z *= MIN_VELOCITY_MULTIPLIER;
                }
            }
        }
    }
}