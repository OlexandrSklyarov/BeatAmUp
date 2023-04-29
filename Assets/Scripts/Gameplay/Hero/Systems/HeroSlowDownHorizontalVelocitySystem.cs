using Leopotam.EcsLite;

namespace BT
{
    public sealed class HeroSlowDownHorizontalVelocitySystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world
                .Filter<CharacterAttack>()
                .Inc<CharacterControllerMovement>()  
                .Inc<CharacterGrounded>()  
                .End();

            var attackPool = world.GetPool<CharacterAttack>();
            var movementPool = world.GetPool<CharacterControllerMovement>();

            foreach(var e in entities)
            {
                ref var attack = ref attackPool.Get(e);
                ref var movement = ref movementPool.Get(e);
                
                if (attack.IsActiveAttack) 
                {                    
                    movement.HorizontalVelocity.x *= ConstPrm.Hero.MIN_VELOCITY_MULTIPLIER;
                    movement.HorizontalVelocity.z *= ConstPrm.Hero.MIN_VELOCITY_MULTIPLIER;
                }
            }
        }
    }
}