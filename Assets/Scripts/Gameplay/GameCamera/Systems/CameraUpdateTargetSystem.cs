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

            var eventEntities = world.Filter<HeroCreatedEvent>().End();

            var heroes = world.
                Filter<Hero>()
                .Inc<CharacterControllerMovement>()
                .End();

            var movementPool = world.GetPool<CharacterControllerMovement>();
            var eventPool = world.GetPool<HeroCreatedEvent>();

            var cam = data.WorldData.GameVirtualCamera;
            var targetGroup = data.WorldData.TargetGroup;

            foreach(var e in eventEntities)
            {
                ref var evt = ref eventPool.Get(e);
                var allTargets = new List<CinemachineTargetGroup.Target>();

                foreach(var h in heroes)
                {
                    var cur = new CinemachineTargetGroup.Target();
                    cur.target = movementPool.Get(h).Transform;
                    cur.weight = 1f / heroes.GetEntitiesCount();
                    allTargets.Add(cur);
                }

                targetGroup.m_Targets = allTargets.ToArray();

                cam.Follow = targetGroup.transform;
                cam.LookAt = targetGroup.transform.GetChild(0);

                var t = cam.GetCinemachineComponent<CinemachineTransposer>();
                t.m_FollowOffset = data.Config.CameraConfig.Offset;

                eventPool.Del(e);
            }

        }
    }
}