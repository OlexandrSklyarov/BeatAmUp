using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class ResetHitCountSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world
                .Filter<CharacterAttack>()
                .Exc<Death>()
                .End();            

            var attackPool = world.GetPool<CharacterAttack>();

            foreach (var e in entities)
            {
                ref var attack = ref attackPool.Get(e);                
                
                if (attack.HitResetTimer > 0f)
                {
                    attack.HitResetTimer -= Time.deltaTime;
                    continue;
                }
                
                attack.HitCount = 0;
                attack.HitResetTimer = 0f;
            }
        }
    }
}