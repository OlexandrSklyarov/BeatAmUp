using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class EnemyCharacterDeactivateRagdollSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
                
            var enemies = world
                .Filter<DeactivateRagdoll>()
                .Inc<RagdollState>()
                .Inc<CharacterView>()
                .Inc<CharacterPhysicsBody>()
                .Exc<Stun>()
                .Exc<Death>()
                .End();

            var ragdollPool = world.GetPool<RagdollState>();
            var deactivateRagdollPool = world.GetPool<DeactivateRagdoll>();
            var viewPool = world.GetPool<CharacterView>();
            var bodyPool = world.GetPool<CharacterPhysicsBody>();
            var deathPool = world.GetPool<Death>();
            var standUpAnimationPool = world.GetPool<StandUpAnimationEvent>();

            foreach (var ent in enemies)
            {
                ref var view = ref viewPool.Get(ent);
                ref var body = ref bodyPool.Get(ent);
                ref var ragdoll = ref ragdollPool.Get(ent);
                ref var deactivateRagdoll = ref deactivateRagdollPool.Get(ent);
                 
                if (!deactivateRagdoll.IsCanStandUp) 
                {
                    deathPool.Add(ent); //death
                    continue;
                }

                ragdoll.RestoreTimer -= Time.deltaTime;

                if (ragdoll.RestoreTimer > 0f)
                {
                    StandUpRagdollProcess(ref ragdoll, ref body);
                    continue;
                }               
                
                ResetRagdoll(ref body, ref view);

                standUpAnimationPool.Add(ent); //stand up anim event  
                ragdollPool.Del(ent);
                deactivateRagdollPool.Del(ent);
            }
        }


        private void StandUpRagdollProcess(ref RagdollState ragdoll, ref CharacterPhysicsBody body)
        {
            var progress = Mathf.Clamp01(1f - ragdoll.RestoreTimer / ConstPrm.Character.RESTORE_RAGDOLL_TIME);

            for(int i = 0; i < body.Bones.Length; i++)
            {
                body.Bones[i].localPosition = Vector3.Lerp(
                    body.RagdollBoneTransforms[i].Position, 
                    body.StandUpBoneTransforms[i].Position, 
                    progress);
               
                body.Bones[i].localRotation = Quaternion.Lerp(
                    body.RagdollBoneTransforms[i].Rotation, 
                    body.StandUpBoneTransforms[i].Rotation, 
                    progress);
            }
        }


        private void ResetRagdoll(ref CharacterPhysicsBody body, ref CharacterView view)
        {
            foreach (var rb in body.BodyRagdoll) rb.isKinematic = true;

            view.Animator.enabled = true;
            body.Collider.enabled = true;
        }        
    }
}