using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Leopotam.EcsLite;

namespace BT
{
    public sealed class CameraUpdateTargetSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var eventEntities = world.Filter<SpawnedHeroEvent>().End();

            var heroes = world.
                Filter<Hero>()
                .Inc<Translation>()
                .End();

            var translationPool = world.GetPool<Translation>();
            var eventPool = world.GetPool<SpawnedHeroEvent>();

            var cam = data.WorldData.GameVirtualCamera;
            var targetGroup = data.WorldData.TargetGroup;

            foreach(var e in eventEntities)
            {
                var allTargets = new List<CinemachineTargetGroup.Target>();

                foreach(var h in heroes)
                {
                    var cur = new CinemachineTargetGroup.Target()
                    {
                        target = translationPool.Get(h).Value,
                        weight = 1f / heroes.GetEntitiesCount()
                    };
                    
                    allTargets.Add(cur);
                }

                targetGroup.m_Targets = allTargets.ToArray();

                cam.Follow = targetGroup.transform;
                cam.LookAt = targetGroup.transform.GetChild(0);

                var t = cam.GetCinemachineComponent<CinemachineTransposer>();
                t.m_FollowOffset = data.Config.CameraConfig.Offset;
            }

        }
    }
}