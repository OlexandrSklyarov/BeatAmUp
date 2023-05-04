using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public class CharacterUnBlockMovementSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var filter = world.Filter<BlockMovement>().End();

            var blockPool = world.GetPool<BlockMovement>();

            foreach(var ent in filter)
            {
                ref var block = ref blockPool.Get(ent);

                block.Timer -= Time.deltaTime;

                if (block.Timer > 0f) continue;

                blockPool.Del(ent);
            }
        }
    }
}