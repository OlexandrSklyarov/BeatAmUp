using Cinemachine;
using Leopotam.EcsLite;

namespace BT
{
    public sealed class InitCameraSystem : IEcsInitSystem
    {
        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var cameraEntity = world.NewEntity();
            var cameraPool = world.GetPool<GameCamera>();
            ref var camera = ref cameraPool.Add(cameraEntity);

            camera.Shake = data.WorldData.GameVirtualCamera
                .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }        
    }
}