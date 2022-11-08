using Leopotam.EcsLite;
using Services.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Character.Hero
{
    public sealed class PlayerInputSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private Vector2 _direction;
        private bool _isJump;
        private bool _isMoved;
        private bool _isRunning;

        public void Init(IEcsSystems systems) 
        {
            var control = systems.GetShared<SharedData>().InputServices;

            control.Enable();

            control.Player.Movement.performed += SetDirection;
            control.Player.Movement.canceled += ReleaseDirection;
            control.Player.Jump.started += ActiveJump;
            control.Player.Running.started += ActiveRunning;
            control.Player.Running.canceled += ActiveRunning;
        }


        public void Destroy(IEcsSystems systems)
        {
            var control = systems.GetShared<SharedData>().InputServices;

            control.Player.Movement.performed -= SetDirection;
            control.Player.Movement.canceled -= ReleaseDirection;
            control.Player.Jump.started -= ActiveJump;
            control.Player.Running.started -= ActiveRunning;
            control.Player.Running.canceled -= ActiveRunning;

            control.Disable();
        }
        

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world
                .Filter<PlayerInputData>()
                .Inc<Movement>()
                .Inc<HeroTag>()
                .End();

            var inputDataPool = world.GetPool<PlayerInputData>();
            var movementPool = world.GetPool<Movement>();

            foreach(var e in entities)
            {
                ref var input = ref inputDataPool.Get(e); 
                ref var movement = ref movementPool.Get(e);       

                var relativeDirection = movement.Transform
                    .TransformDirection(new Vector3(_direction.x, 0f, _direction.y));

                input.Direction = relativeDirection;
                input.IsMoved = _isMoved;
                input.IsJump = _isJump;
                input.IsRunning = _isRunning;

                _isJump = false;
            }
        }


        private void ActiveJump(InputAction.CallbackContext ctx) => _isJump = ctx.ReadValue<float>() > 0f;


        private void ActiveRunning(InputAction.CallbackContext ctx) => _isRunning = ctx.ReadValue<float>() > 0f;
        

        private void SetDirection(InputAction.CallbackContext ctx) 
        {
            _direction = ctx.ReadValue<Vector2>();
            _isMoved = true;
        }


        private void ReleaseDirection(InputAction.CallbackContext ctx)
        {
            _direction = ctx.ReadValue<Vector2>();
            _isMoved = false;
        }
    }
}