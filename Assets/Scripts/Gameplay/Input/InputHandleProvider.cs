using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Input
{
    public class InputHandleProvider
    {
        public Vector2 Direction {get; private set;}
        public bool IsJump {get; private set;}
        public bool IsMoved {get; private set;}
        public bool IsRunning {get; private set;}
        public bool IsKick {get; private set;}
        public bool IsPunch {get; private set;}
        
        private readonly InputServices _control;


        public InputHandleProvider(InputServices control)
        {
            _control = control;
        }


        public void ResetInput()
        {
            IsJump = IsKick = IsPunch = false;            
        }


        public void Enable()
        {
            _control.Enable();

            _control.Player.Movement.performed += SetDirection;
            _control.Player.Movement.canceled += ReleaseDirection;
            _control.Player.Jump.started += ActiveJump;
            _control.Player.Running.started += ActiveRunning;
            _control.Player.Running.canceled += ActiveRunning;
            _control.Player.Kick.started += ActiveKick;
            _control.Player.Punch.started += ActivePunch;
        }


        public void Disable()
        {
            _control.Player.Movement.performed -= SetDirection;
            _control.Player.Movement.canceled -= ReleaseDirection;
            _control.Player.Jump.started -= ActiveJump;
            _control.Player.Running.started -= ActiveRunning;
            _control.Player.Running.canceled -= ActiveRunning;
            _control.Player.Kick.started -= ActiveKick;
            _control.Player.Punch.started -= ActivePunch;

            _control.Disable();
        }


        private void ActivePunch(InputAction.CallbackContext ctx) => IsPunch = ctx.ReadValue<float>() > 0f;
    

        private void ActiveKick(InputAction.CallbackContext ctx) => IsKick = ctx.ReadValue<float>() > 0f;


        private void ActiveJump(InputAction.CallbackContext ctx) => IsJump = ctx.ReadValue<float>() > 0f;


        private void ActiveRunning(InputAction.CallbackContext ctx) => IsRunning = ctx.ReadValue<float>() > 0f;
        

        private void SetDirection(InputAction.CallbackContext ctx) 
        {
            Direction = ctx.ReadValue<Vector2>();
            IsMoved = true;
        }


        private void ReleaseDirection(InputAction.CallbackContext ctx)
        {
            Direction = ctx.ReadValue<Vector2>();
            IsMoved = false;
        }
    }
}