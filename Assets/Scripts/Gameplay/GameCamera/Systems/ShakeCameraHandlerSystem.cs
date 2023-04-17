using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public class ShakeCameraHandlerSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var cameraEntities = world.
                Filter<GameCamera>() 
                .Inc<ShakeCameraEvent>()
                .End(); 

            var cameraPool = world.GetPool<GameCamera>();
            var shakeEventPool = world.GetPool<ShakeCameraEvent>();

            foreach (var ent in cameraEntities)
            {
                ref var camera = ref cameraPool.Get(ent);
                ref var evt = ref shakeEventPool.Get(ent);

                if (evt.Timer > 0f)
                {
                    evt.Timer -= Time.deltaTime;

                    camera.Shake.m_AmplitudeGain = data.Config.CameraConfig.CameraShakeAmplitude;
                    camera.Shake.m_FrequencyGain = data.Config.CameraConfig.CameraShakeFrequency;

                    continue;
                }

                camera.Shake.m_FrequencyGain = 0f;
                shakeEventPool.Del(ent);
            }
        }
    }
}