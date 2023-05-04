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
                .Filter<RestoreRagdollState>()
                .Inc<RagdollState>()
                .Inc<CharacterView>()
                .Inc<CharacterPhysicsBody>()
                .Exc<Stun>()
                .Exc<Death>()
                .End();

            var ragdollPool = world.GetPool<RagdollState>();
            var deactivateRagdollPool = world.GetPool<RestoreRagdollState>();
            var viewPool = world.GetPool<CharacterView>();
            var bodyPool = world.GetPool<CharacterPhysicsBody>();
            var deathPool = world.GetPool<Death>();
            var standUpAnimationPool = world.GetPool<StandUpAnimationEvent>();

            foreach (var ent in enemies)
            {
                ref var view = ref viewPool.Get(ent);
                ref var body = ref bodyPool.Get(ent);
                ref var ragdoll = ref ragdollPool.Get(ent);
                ref var restoreRagdoll = ref deactivateRagdollPool.Get(ent);
                 
                if (!restoreRagdoll.IsCanStandUp) 
                {
                    deathPool.Add(ent); //death
                    continue;
                }

                ragdoll.RestoreTimer -= Time.deltaTime;

                if (ragdoll.RestoreTimer > 0f)
                {
                    StandUpRagdollProcess(ref ragdoll, ref restoreRagdoll, ref body);
                    continue;
                }               
                
                view.Animator.enabled = true;
                body.Collider.enabled = true;

                ref var animEvent = ref standUpAnimationPool.Add(ent); //stand up anim event  
                animEvent.IsFaceDown = restoreRagdoll.IsFaceDown;
                
                ragdollPool.Del(ent);
                deactivateRagdollPool.Del(ent);
            }
        }


        private void StandUpRagdollProcess(ref RagdollState ragdoll, ref RestoreRagdollState restoreRagdoll,
            ref CharacterPhysicsBody body)
        {
            var progress = Mathf.Clamp01(1f - ragdoll.RestoreTimer / ConstPrm.Character.RESTORE_RAGDOLL_TIME);

            var targetBones = (restoreRagdoll.IsFaceDown) ? 
                body.StandUpFaceDownBoneTransforms : 
                body.StandUpFaceBoneTransforms;
                
            for(int i = 0; i < body.Bones.Length; i++)
            {
                body.Bones[i].localPosition = Vector3.Lerp(
                    body.RagdollBoneTransforms[i].Position, 
                    targetBones[i].Position, 
                    progress);
               
                body.Bones[i].localRotation = Quaternion.Lerp(
                    body.RagdollBoneTransforms[i].Rotation, 
                    targetBones[i].Rotation, 
                    progress);
            }
        }
    }
}