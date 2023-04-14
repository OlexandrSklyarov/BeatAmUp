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
            var ragdollStatePool = world.GetPool<RagdollState>();
            var resetRagdollPool = world.GetPool<DeactivateRagdollEvent>();

            foreach (var ent in entities)
            {
                ref var stun = ref stunPool.Get(ent);
                stun.Timer -= Time.deltaTime;
                var isStunEnd = stun.Timer <= 0f;

                if (isStunEnd)
                {
                    var isNeedResetRagdoll = ragdollStatePool.Has(ent) && !resetRagdollPool.Has(ent);
                    if (isNeedResetRagdoll) resetRagdollPool.Add(ent);
                    
                    stunPool.Del(ent); 
                }               
            }
        }
    }
}