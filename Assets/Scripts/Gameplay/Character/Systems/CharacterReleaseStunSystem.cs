using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class CharacterReleaseStunSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world
                .Filter<Stun>()
                .Exc<Death>()
                .End();

            var stunPool = world.GetPool<Stun>();

            foreach (var e in entities)
            {
                ref var stun = ref stunPool.Get(e);
                
                stun.Timer -= Time.deltaTime;

                if (stun.Timer <= 0f)
                {
                    stunPool.Del(e);
                }   
            }
        }
    }
}