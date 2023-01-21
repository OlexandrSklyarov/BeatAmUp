using System.Data;
using Leopotam.EcsLite;

namespace BT
{
    public sealed class ResetPlayerInputDataSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world  = systems.GetWorld();
            var control = systems.GetShared<SharedData>().InputProvider;

            var entities = world
                .Filter<HeroAttack>()
                .Inc<HeroTag>()
                .Inc<CharacterCommand>()
                .Inc<Movement>()
                .End();

            var attackPool = world.GetPool<HeroAttack>();
            var commandPool = world.GetPool<CharacterCommand>();
            var groundedPool = world.GetPool<CharacterGrounded>();
            var movementPool = world.GetPool<Movement>();

            foreach(var e in entities)
            {
                control.ResetInput();

                ref var attack = ref attackPool.Get(e);
                attack.CurrentKick = null;
                attack.CurrentPunch = null;

                ref var command = ref commandPool.Get(e);
                ref var movement = ref movementPool.Get(e);  
                var isHasGrounded = groundedPool.Has(e);

                if (!command.IsJump && isHasGrounded) 
                {
                    movement.IsJumpProcess = false;
                }
            }
        }
    }
}