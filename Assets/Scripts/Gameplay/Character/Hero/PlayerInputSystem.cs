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


        public void Init(IEcsSystems systems) 
        {
            var control = systems.GetShared<SharedData>().InputServices;
            control.Enable();
            control.Player.Movement.performed += SetDirection;
            control.Player.Jump.performed += ActiveJump;
        }


        public void Run(IEcsSystems systems)
        {
            var entities = systems.GetWorld().Filter<PlayerInputData>().End();
            var inputDataPool = systems.GetWorld().GetPool<PlayerInputData>();

            foreach(var e in entities)
            {
                ref var input = ref inputDataPool.Get(e);        

                input.Direction = new Vector3(_direction.x, 0f, _direction.y);
                input.Jump = _isJump;

                _isJump = false;
            }
        }


        public void Destroy(IEcsSystems systems)
        {
            var control = systems.GetShared<SharedData>().InputServices;
            control.Player.Movement.performed -= SetDirection;
            control.Player.Jump.performed -= ActiveJump;
            control.Disable();
        }


        private void ActiveJump(InputAction.CallbackContext obj) => _isJump = true;


        private void SetDirection(InputAction.CallbackContext ctx) => _direction = ctx.ReadValue<Vector2>();
    }
}