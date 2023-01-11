using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class CharacterRotateViewSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var config = systems.GetShared<SharedData>().Config;

            var entities = world.Filter<CharacterCommand>()
                .Inc<CharacterView>()
                .End();
                
            var inputPool = world.GetPool<CharacterCommand>();
            var viewPool = world.GetPool<CharacterView>();
            var movementPool = world.GetPool<Movement>();

            foreach (var e in entities)
            {
                ref var input = ref inputPool.Get(e);
                ref var view = ref viewPool.Get(e);
                ref var move = ref movementPool.Get(e);

                if (!input.IsMoved) continue;

                var dir = move.HorizontalVelocity.normalized;
                var angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                var targetRotation = Quaternion.Euler(new Vector3(0f, angle, 0f));

                view.ViewTransform.rotation = Quaternion.RotateTowards
                (
                    view.ViewTransform.rotation,
                    targetRotation,
                    Time.deltaTime * config.PlayerData.RotateSpeed
                );

            }
        }
    }
}