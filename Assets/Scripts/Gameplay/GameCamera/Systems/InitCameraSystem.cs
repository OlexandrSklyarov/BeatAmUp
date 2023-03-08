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

            CreateCameraEntity(world, data);

            var entities = world.
                Filter<HeroTag>()
                .Inc<CharacterControllerMovement>()
                .End();

            var movementPool = world.GetPool<CharacterControllerMovement>();

            foreach(var e in entities)
            {
                var provider = movementPool.Get(e).Transform.GetComponent<HeroViewProvider>();

                data.WorldData.GameVirtualCamera.LookAt = provider.CameraLookPoint;
                data.WorldData.GameVirtualCamera.Follow = provider.CameraFollowPoint;

                var t = data.WorldData.GameVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
                t.m_FollowOffset = data.Config.CameraConfig.Offset;
            }
        }


        private void CreateCameraEntity(EcsWorld world, SharedData data)
        {
            var cameraEntity = world.NewEntity();
            var cameraPool = world.GetPool<GameCamera>();
            ref var camera = ref cameraPool.Add(cameraEntity);

            camera.Shake = data.WorldData.GameVirtualCamera
                .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }
}