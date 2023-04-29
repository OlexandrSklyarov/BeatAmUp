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
                .Filter<TakeDamageEvent>()
                .Inc<CharacterPhysicsBody>()
                .Inc<CharacterView>()
                .End();

            var damageEventPool = world.GetPool<TakeDamageEvent>();
            var ragdollStatePoll = world.GetPool<RagdollState>();
            var physicsBodyPool = world.GetPool<CharacterPhysicsBody>();
            var viewPool = world.GetPool<CharacterView>();

            foreach (var ent in entities)
            {
                ref var damageEvt = ref damageEventPool.Get(ent);
                ref var body = ref physicsBodyPool.Get(ent);
                ref var view = ref viewPool.Get(ent);

                var isActiveRagdoll = ragdollStatePoll.Has(ent);
                
                if (!IsCanActiveRagdoll(ref damageEvt, isActiveRagdoll)) continue;

                AddForceCharacterRagdoll(ref damageEvt, ref body, ref view);
                
                if (!isActiveRagdoll) ragdollStatePoll.Add(ent);
            }
        }
        

        private bool IsCanActiveRagdoll(ref TakeDamageEvent damageEvt, bool isRagdollEnabled)
        {
            return isRagdollEnabled || (damageEvt.IsPowerDamage && !damageEvt.IsHammeringDamage);
        }


        private void AddForceCharacterRagdoll(ref TakeDamageEvent damageEvt,
            ref CharacterPhysicsBody body, ref CharacterView view)
        {
            Rigidbody targetRb = body.BodyRagdoll.First();
            var minSqDist = float.MaxValue;
            
            foreach (var rb in body.BodyRagdoll)
            {
                rb.isKinematic = false;
                var curSqDist = (rb.transform.position - damageEvt.HitPoint).sqrMagnitude;

                if (curSqDist < minSqDist)
                {
                    minSqDist = curSqDist;
                    targetRb = rb;
                }
            }
            
            view.Animator.enabled = false;
            body.Collider.enabled = false;

            var force = damageEvt.HitDirection * damageEvt.PushForce;
            targetRb.AddForceAtPosition(force, targetRb.position, ForceMode.Impulse);
        }
    }
}
