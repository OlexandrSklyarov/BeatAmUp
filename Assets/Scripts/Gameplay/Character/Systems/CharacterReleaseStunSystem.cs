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
                .Exc<DeactivateRagdollEvent>()
                .End();

            var stunPool = world.GetPool<Stun>();
            var rdStatePool = world.GetPool<RagdollState>();
            var deactivateRdPool = world.GetPool<DeactivateRagdollEvent>();

            foreach (var ent in entities)
            {
                ref var stun = ref stunPool.Get(ent);
                stun.Timer -= Time.deltaTime;
                var isStunEnd = stun.Timer <= 0f;

                if (isStunEnd)
                {
                    var isActiveRagdoll = rdStatePool.Has(ent);
                    if (isActiveRagdoll) deactivateRdPool.Add(ent);
                    
                    stunPool.Del(ent); 
                }               
            }
        }
    }
}