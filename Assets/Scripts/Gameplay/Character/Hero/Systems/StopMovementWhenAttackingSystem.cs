using UnityEngine;
using Leopotam.EcsLite;

namespace BT
{
    public sealed class StopMovementWhenAttackingSystem : IEcsRunSystem
    {
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

                if (attack.IsActiveAttack) movement.HorizontalVelocity = Vector3.zero;
            }
        }
    }
}