using System.Linq;
using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class CharacterActiveRagdollSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world
                .Filter<ActiveRagdollEvent>()
                .Inc<CharacterPhysicsBody>()
                .Inc<CharacterView>()
                .Exc<RagdollState>()
                .End();

            var ragdollEventPool = world.GetPool<ActiveRagdollEvent>();
            var ragdollStatePoll = world.GetPool<RagdollState>();
            var physicsBodyPool = world.GetPool<CharacterPhysicsBody>();
            var viewPool = world.GetPool<CharacterView>();

            foreach (var ent in entities)
            {
                ref var ragdollEvent = ref ragdollEventPool.Get(ent);
                ref var body = ref physicsBodyPool.Get(ent);
                ref var view = ref viewPool.Get(ent);

                if (!ragdollStatePoll.Has(ent))
                {
                    ActiveCharacterRagdoll(ref ragdollEvent, ref body, ref view);
                    ragdollStatePoll.Add(ent);
                }

                ragdollEventPool.Del(ent);
            }
        }
        

        private void ActiveCharacterRagdoll(ref ActiveRagdollEvent ragdollEvent, 
            ref CharacterPhysicsBody body, ref CharacterView view)
        {
            view.Animator.enabled = false;
            
            Rigidbody nearestPart = body.BodyRagdoll.First();
            var minDist = float.MaxValue;
            
            foreach (var rb in body.BodyRagdoll)
            {
                rb.isKinematic = false;
                var curSqDist = (rb.transform.position - ragdollEvent.NearDamagePoint).sqrMagnitude;

                if (curSqDist < minDist * minDist)
                {
                    minDist = curSqDist;
                    nearestPart = rb;
                }
            }

            var force = ragdollEvent.PushDirection * 50f;
            nearestPart.AddForce(force, ForceMode.Impulse);
        }
    }
}