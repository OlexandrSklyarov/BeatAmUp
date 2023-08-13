using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using BT;
using UnityEngine.InputSystem.Users;

namespace Services.Input
{
    public class PlayerBindInputService
    {
        public bool IsHasFreeController => _controlItems.Any(x => !x.IsUsed);

        public int DeviceCount => _controlItems.Count;

        private readonly List<ControlItem> _controlItems = new();


        public PlayerBindInputService(GameRules _gameRules)
        {
            Util.Debug.PrintColor($"PlayerBindInputService: {InputSystem.devices.Count}", UnityEngine.Color.green);

            var gameDevices = GetDevices(_gameRules.ControlType);

            ShowDebugDeviceInfo(gameDevices, _gameRules.MaxPlayerCount);         

            for (int i = 0; i < gameDevices.Length; i++)
            {
                Create(gameDevices[i]);
            }
        }


        private void Create(InputDevice device)
        {
            var action = new InputServices();

            _controlItems.Add(new ControlItem()
            {
                Action = action,

                Controller = new InputController()
                {
                    InputProvider = new InputHandleProvider(action),
                    Device = device,
                    User = InputUser.PerformPairingWithDevice(device)
                }
            });
        }


        private InputDevice[] GetDevices(ControlDeviceType controlType)
        {
            var allDevices = InputSystem.devices;

            return controlType switch
            {
                ControlDeviceType.KEYBOARD => allDevices
                    .Where(k => k.name == ConstPrm.DevicesName.KEYBOARD)
                    .ToArray(),

                ControlDeviceType.GAMEPAD => allDevices
                    .Where(g => g.name == ConstPrm.DevicesName.GAMEPAD)
                    .ToArray(),

                _ => allDevices
                    .Where(d => d.name == ConstPrm.DevicesName.KEYBOARD || d.name == ConstPrm.DevicesName.GAMEPAD)
                    .ToArray(),
            };
        }


        private void ShowDebugDeviceInfo(InputDevice[] gameDevices, int maxPlayerCount)
        {            
            var colorValue = 0.5f;
            var addValue = 1f / maxPlayerCount;
            
            Array.ForEach(gameDevices, d =>
            {
                var color = new UnityEngine.Color(0.7f, colorValue, 0.7f);
                Util.Debug.PrintColor($" ++++++++++++++++++++++", color);    
                Util.Debug.PrintColor($"device: {d.deviceId}", color);
                Util.Debug.PrintColor($"device: {d.description}", color);
                Util.Debug.PrintColor($"device: {d.name}", color);
                Util.Debug.PrintColor($" ++++++++++++++++++++++", color);    

                colorValue += addValue;            
            });
        }


        public void BindDeviceToUser(ref HeroInputUser inputUser)
        {           
            if (IsHasFreeController)
            {
                var item = _controlItems.First(x => !x.IsUsed);

                var deviceName = item.Controller.Device.name;
                var action = item.Action;

                item.Controller.User.ActivateControlScheme(action.controlSchemes.First(s => s.name.Equals(deviceName)));
                item.Controller.User.AssociateActionsWithUser(action);
                item.IsUsed = true;

                inputUser.Controller = item.Controller;
                return;
            }

            throw new Exception("No free device to bind the user!!!");
        }


        public bool IsHasFreeControllers(int count) => _controlItems.Count(x => !x.IsUsed) >= count;


        private class ControlItem
        {
            public InputServices Action; 
            public InputController Controller;
            public bool IsUsed;
        }
    }
}