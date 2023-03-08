using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public class ShakeCameraSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var cameraEntities = world.Filter<GameCamera>() .End();
            var eventEntities = world.Filter<ShakeCameraEvent>().End();

            var cameraPool = world.GetPool<GameCamera>();
            var shakeEventPool = world.GetPool<ShakeCameraEvent>();

            foreach(var c in cameraEntities)
            {
                ref var camera = ref cameraPool.Get(c);

                foreach(var e in eventEntities)
                {
                    ref var evt = ref shakeEventPool.Get(e);

                    if (evt.Timer > 0f)
                    {
                        evt.Timer -=Time.deltaTime;

                        camera.Shake.m_AmplitudeGain = data.Config.CameraConfig.CameraShakeAmplitude;
                        camera.Shake.m_FrequencyGain = data.Config.CameraConfig.CameraShakeFrequency;

                        continue;
                    }

                    camera.Shake.m_FrequencyGain = 0f;
                    shakeEventPool.Del(e);
                }
            }
        }
    }
}