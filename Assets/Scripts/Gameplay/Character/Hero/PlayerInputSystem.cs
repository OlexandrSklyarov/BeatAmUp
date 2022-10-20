using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Services.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Character.Hero
{
    public sealed class PlayerInputSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private Vector2 _direction;
        private bool _isJump;


        public void Init(IEcsSystems systems) 
        {
            var control = systems.GetShared<SharedData>().InputServices;

            control.Enable();

            control.Player.Movement.performed += SetDirection;
            control.Player.Movement.canceled += SetDirection;

            control.Player.Jump.started += ActiveJump;
            control.Player.Jump.canceled += ActiveJump;
        }


        public void Run(IEcsSystems systems)
        {
            var entities = systems.GetWorld()
                .Filter<PlayerInputData>()
                .Inc<HeroTag>()
                .End();

            var inputDataPool = systems.GetWorld().GetPool<PlayerInputData>();

            foreach(var e in entities)
            {
                ref var input = ref inputDataPool.Get(e);        

                input.Direction = new Vector3(_direction.x, 0f, _direction.y);
                input.IsJump = _isJump;
            }
        }


        public void Destroy(IEcsSystems systems)
        {
            var control = systems.GetShared<SharedData>().InputServices;

            control.Player.Movement.performed -= SetDirection;
            control.Player.Movement.canceled -= SetDirection;

            control.Player.Jump.started -= ActiveJump;
            control.Player.Jump.canceled -= ActiveJump;

            control.Disable();
        }


        private void ActiveJump(InputAction.CallbackContext ctx) => _isJump = ctx.ReadValue<float>() > 0f;


        private void SetDirection(InputAction.CallbackContext ctx) => _direction = ctx.ReadValue<Vector2>();
    }
}