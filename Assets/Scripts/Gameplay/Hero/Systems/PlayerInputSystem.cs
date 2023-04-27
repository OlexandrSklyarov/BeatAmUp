using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class PlayerInputSystem : IEcsRunSystem, IEcsDestroySystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world
                .Filter<Hero>()
                .Inc<HeroInputUser>()
                .Inc<CharacterCommand>()
                .End();

            var inputPool = world.GetPool<HeroInputUser>();
            var commandPool = world.GetPool<CharacterCommand>();

            foreach(var e in entities)
            {
                ref var command = ref commandPool.Get(e); 

                ref var input = ref inputPool.Get(e); 
                var provider = input.InputProvider;

                command.IsKick = provider.IsKick;
                command.IsPunch = provider.IsPunch;

                var isAttack = (command.IsKick || command.IsPunch);

                command.Direction = GetDirection(provider, isAttack);

                command.IsMoved = !isAttack && provider.IsMoved;
                command.IsJump = provider.IsJump;
                command.IsRunning = provider.IsRunning;
                command.IsSitting = provider.IsSitting;
                
                provider.ResetInputValue();
            }
        }
        

        private Vector3 GetDirection(InputHandleProvider provider, bool isAttack)
        {
            return (isAttack) ? Vector3.zero : new Vector3(provider.Direction.x, 0f, provider.Direction.y);
        }
        
        
        public void Destroy(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var entities = world.Filter<HeroInputUser>().End();
            var inputPool = world.GetPool<HeroInputUser>();

            foreach(var e in entities)
            {
                ref var input = ref inputPool.Get(e);
                input.InputProvider.Disable();            
            }
        }
    }
}
