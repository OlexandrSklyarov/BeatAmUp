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
                .Inc<Movement>()
                .Inc<HeroTag>()
                .End();

            var inputDataPool = world.GetPool<CharacterCommand>();
            var movementPool = world.GetPool<Movement>();

            foreach(var e in entities)
            {
                ref var input = ref inputDataPool.Get(e); 
                ref var movement = ref movementPool.Get(e);       

                var relativeDirection = movement.Transform
                    .TransformDirection(new Vector3(control.Direction.x, 0f, control.Direction.y));

                input.IsKick = control.IsKick;
                input.IsPunch = control.IsPunch;

                var isAttack = (input.IsKick || input.IsPunch);

                input.Direction = (!isAttack) ? relativeDirection : Vector3.zero;
                input.IsMoved = !isAttack && control.IsMoved;
                input.IsJump = control.IsJump;
                input.IsRunning = control.IsRunning;
                input.IsSitting = control.IsSitting;

                control.ResetInput();                
            }
        }        
    }
}