using Leopotam.EcsLite;
using UnityEngine.InputSystem;

namespace BT
{
    public sealed class InitWorldSystem : IEcsInitSystem
    {
        public void Init(IEcsSystems systems)
        {
            Util.Debug.Print("Init world...");

            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();
            
            //first player
            Util.Debug.PrintColor($"InputSystem.devices: {InputSystem.devices.Count}", UnityEngine.Color.green);
            System.Array.ForEach(InputSystem.devices.ToArray(), d => Util.Debug.PrintColor($"device: {d.deviceId}", UnityEngine.Color.green));
            
            var currentDevice = InputSystem.devices[0];
            CreateHeroRequest(world, currentDevice, data);

            InputSystem.onDeviceChange += (device, change) =>
            {
                switch (change)
                {
                    case InputDeviceChange.Added:
                        Util.Debug.PrintColor($"New device added: {device}", UnityEngine.Color.green);
                        CreateHeroRequest(world, device, data);
                        break;

                    case InputDeviceChange.Removed:
                        Util.Debug.PrintColor($"Device removed: {device}", UnityEngine.Color.magenta);
                        break;
                }
            };            
        }


        private void CreateHeroRequest(EcsWorld world, InputDevice device, SharedData data)
        {
            var heroCount = world.Filter<HeroTag>().End().GetEntitiesCount();

            if (heroCount > data.Config.MaxPlayerCount) 
            {
                Util.Debug.PrintColor($"The player limit in the game has been exceeded, count: {heroCount}", UnityEngine.Color.yellow);
                return;
            }

            var requestEntity = world.NewEntity();
            var pool = world.GetPool<CreateHeroRequest>(); 
                       
            ref var request = ref pool.Add(requestEntity);            
            request.HeroID = heroCount;
            request.Device = device;
        }
    }
}