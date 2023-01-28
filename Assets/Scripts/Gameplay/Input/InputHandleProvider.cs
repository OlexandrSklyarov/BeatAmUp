using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BT
{
    public class InputHandleProvider
    {
        public Vector2 Direction {get; private set;}
        public bool IsJump {get; private set;}
        public bool IsMoved {get; private set;}
        public bool IsRunning {get; private set;}
        public bool IsKick {get; private set;}
        public bool IsPunch {get; private set;}
        public bool IsSitting { get; private set; }

        private readonly InputServices _control;


        public InputHandleProvider(InputServices control)
        {
            _control = control;
        }


        public void ResetInput()
        {
            IsKick = IsPunch = false;            
        }


        public void Enable()
        {
            _control.Enable();

            _control.Player.Movement.performed += SetDirection;
            _control.Player.Movement.canceled += ReleaseDirection;
            _control.Player.Jump.started += ActiveJump;
            _control.Player.Jump.canceled += ActiveJump;
            _control.Player.Running.started += ActiveRunning;
            _control.Player.Running.canceled += ActiveRunning;
            _control.Player.Kick.started += ActiveKick;
            _control.Player.Punch.started += ActivePunch;
            _control.Player.Sitting.started += ActiveSitting;
            _control.Player.Sitting.canceled += ActiveSitting;
        }


        public void Disable()
        {
            _control.Player.Movement.performed -= SetDirection;
            _control.Player.Movement.canceled -= ReleaseDirection;
            _control.Player.Jump.started -= ActiveJump;
            _control.Player.Jump.canceled -= ActiveJump;
            _control.Player.Running.started -= ActiveRunning;
            _control.Player.Running.canceled -= ActiveRunning;
            _control.Player.Kick.started -= ActiveKick;
            _control.Player.Punch.started -= ActivePunch;
            _control.Player.Sitting.started -= ActiveSitting;
            _control.Player.Sitting.canceled -= ActiveSitting;

            _control.Disable();
        }


        private void ActiveSitting(InputAction.CallbackContext ctx) => IsSitting = ctx.ReadValueAsButton();


        private void ActivePunch(InputAction.CallbackContext ctx) => IsPunch = ctx.ReadValueAsButton();
    

        private void ActiveKick(InputAction.CallbackContext ctx) => IsKick = ctx.ReadValueAsButton();


        private void ActiveJump(InputAction.CallbackContext ctx) => IsJump = ctx.ReadValueAsButton();


        private void ActiveRunning(InputAction.CallbackContext ctx) => IsRunning = ctx.ReadValueAsButton();
        

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