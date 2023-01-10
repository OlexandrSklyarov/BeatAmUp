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

            var entities = world.
                Filter<HeroTag>()
                .Inc<Movement>()
                .End();

            var movementPool = world.GetPool<Movement>();

            foreach(var e in entities)
            {
                var provider = movementPool.Get(e).Transform.GetComponent<HeroViewProvider>();

                data.WorldData.GameVC.LookAt = provider.CameraLookPoint;
                data.WorldData.GameVC.Follow = provider.CameraFollowPoint;

                var t = data.WorldData.GameVC.GetCinemachineComponent<CinemachineTransposer>();
                t.m_FollowOffset = data.Config.CameraConfig.Offset;
            }
        }
    }
}