using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class PlayerInputSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {        
        public void Init(IEcsSystems systems) 
        {
            var control = systems.GetShared<SharedData>().InputProvider;
            control.Enable();
            control.ResetInput(); 
        }


        public void Destroy(IEcsSystems systems)
        {
            var control = systems.GetShared<SharedData>().InputProvider;
            control.Disable();            
        }
        

        public void Run(IEcsSystems systems)
        {
            var control = systems.GetShared<SharedData>().InputProvider;
            var world = systems.GetWorld();

            var entities = world
                .Filter<CharacterCommand>()
                .Inc<CharacterControllerMovement>()
                .Inc<HeroTag>()
                .End();

            var commandPool = world.GetPool<CharacterCommand>();
            var movementPool = world.GetPool<CharacterControllerMovement>();

            foreach(var e in entities)
            {
                ref var command = ref commandPool.Get(e); 
                ref var movement = ref movementPool.Get(e);       

                var relativeDirection = movement.Transform
                    .TransformDirection(new Vector3(control.Direction.x, 0f, control.Direction.y));

                command.IsKick = control.IsKick;
                command.IsPunch = control.IsPunch;

                var isAttack = (command.IsKick || command.IsPunch);

                movement.Direction = (!isAttack) ? relativeDirection : Vector3.zero;

                command.IsMoved = !isAttack && control.IsMoved;
                command.IsJump = control.IsJump;
                command.IsRunning = control.IsRunning;
                command.IsSitting = control.IsSitting;              

                control.ResetInput();                
            }
        }        
    }
}