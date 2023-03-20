using System;
using System.Linq;
using Leopotam.EcsLite;
using UnityEngine.InputSystem;

namespace BT
{
    public sealed class InitPlayerControlSystem : IEcsInitSystem
    {
        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();
            
            InitPlayerControl(world, data); 
        }


        private void InitPlayerControl(EcsWorld world, SharedData data)
        {
            Util.Debug.PrintColor($"InputSystem.devices: {InputSystem.devices.Count}", UnityEngine.Color.green);

            var gameDevices = InputSystem.devices
                .Where(d => d.name == ConstPrm.DevicesName.KEYBOARD || d.name == ConstPrm.DevicesName.GAMEPAD)
                .ToArray();

            ShowDebugDeviceInfo(gameDevices, data);         

            for (int i = 0; i < gameDevices.Length; i++)
            {
                CreateHeroRequest(world, gameDevices[i], data, i);
            }
        }


        private void ShowDebugDeviceInfo(InputDevice[] gameDevices, SharedData data)
        {
            if (!data.Config.GameDebugConfig.ShowDeviceInfo) return;

            var colorValue = 0.5f;
            var addValue = 1f / data.Config.MaxPlayerCount;
            
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


        private void CreateHeroRequest(EcsWorld world, InputDevice device, SharedData data, int heroID)
        {
            var heroes = world
                .Filter<HeroTag>()
                .Inc<HeroInputUser>()
                .End();

            if (IsLimitPlayersExceeded(heroes, data)) return;

            var requestPool = world.GetPool<CreateHeroRequest>(); 

            if (IsRequestCreated(world, device, requestPool)) return;  
            if (IsInputUserExisted(world, device, heroes)) return;

            var requestEntity = world.NewEntity();                       
            ref var request = ref requestPool.Add(requestEntity);            
            request.HeroID = heroID;
            request.Device = device;
        }


        private bool IsLimitPlayersExceeded(EcsFilter heroes, SharedData data)
        {
            var heroCount = heroes.GetEntitiesCount();

            if (heroCount > data.Config.MaxPlayerCount) 
            {
                Util.Debug.PrintColor($"The player limit in the game has been exceeded, count: {heroCount}", UnityEngine.Color.yellow);
                return true;
            }

            return false;
        }


        private bool IsRequestCreated(EcsWorld world, InputDevice device, EcsPool<CreateHeroRequest> requestPool)
        {
            
            var requestEntities = world.Filter<CreateHeroRequest>().End();
            foreach(var r in requestEntities) 
            {
                ref var curRequest = ref requestPool.Get(r);
                if (curRequest.Device == device) 
                {
                    Util.Debug.PrintColor($"This request created", UnityEngine.Color.yellow);
                    return true;
                }
            }

            return false;
        }


        private bool IsInputUserExisted(EcsWorld world, InputDevice device, EcsFilter heroes)
        {
            var userPool = world.GetPool<HeroInputUser>();

            foreach(var h in heroes)
            {
                ref var user = ref userPool.Get(h);

                if (user.Device.name == device.name)
                {
                    Util.Debug.PrintColor($"This user existed device{device}: {heroes.GetEntitiesCount()}", UnityEngine.Color.yellow);
                    return true;
                } 
            }

            return false;
        }
    }
}