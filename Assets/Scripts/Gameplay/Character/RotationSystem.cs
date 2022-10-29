using System.Collections;
using System.Collections.Generic;
using Gameplay.Character.Hero;
using Leopotam.EcsLite;
using Services.Data;
using UnityEngine;

namespace Gameplay.Character
{
    public class RotationSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var config = systems.GetShared<SharedData>().Config;

            var entities = world.Filter<PlayerInputData>().End();
            var inputPool = world.GetPool<PlayerInputData>();
            var movementPool = world.GetPool<Movement>();

            foreach (var e in entities)
            {
                ref var input = ref inputPool.Get(e);
                ref var movement = ref movementPool.Get(e);

                if (!input.IsMoved) continue;

                var angle = Mathf.Atan2(input.Direction.x, input.Direction.z) * Mathf.Rad2Deg;
                var targetRotation = Quaternion.Euler(new Vector3(0f, angle, 0f));

                movement.ViewTransform.rotation = Quaternion.RotateTowards
                (
                    movement.ViewTransform.rotation,
                    targetRotation,
                    Time.deltaTime * config.PlayerData.RotateSpeed
                );

            }
        }
    }
}