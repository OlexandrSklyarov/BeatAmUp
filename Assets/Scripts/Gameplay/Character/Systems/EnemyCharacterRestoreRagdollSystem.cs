using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class EnemyCharacterRestoreRagdollSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

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

                if (!IsStandUpProcessCompleted(ref ragdoll, ref restoreRagdoll, ref body)) continue;

                ref var evt = ref standUpAnimationPool.Add(ent); //stand up anim event  
                evt.IsFaceDown = restoreRagdoll.IsFaceDown;

                view.Animator.enabled = true;
                body.Collider.enabled = true;

                ragdollPool.Del(ent);
                deactivateRagdollPool.Del(ent);                
            }
        }


        private bool IsStandUpProcessCompleted(ref RagdollState ragdoll, ref RestoreRagdollState restoreRagdoll,
            ref CharacterPhysicsBody body)
        {
            ragdoll.RestoreTimer -= Time.deltaTime;

            if (ragdoll.RestoreTimer > 0f)
            {
                StandUpRagdollProcess(ref ragdoll, ref restoreRagdoll, ref body);
                return false;
            }

            return true;
        }


        private void StandUpRagdollProcess(ref RagdollState ragdoll, 
        ref RestoreRagdollState restoreRagdoll, ref CharacterPhysicsBody body)
        {
            var progress = Mathf.Clamp01(1f - ragdoll.RestoreTimer / ConstPrm.Character.RESTORE_RAGDOLL_TIME);

            var targetBones = (restoreRagdoll.IsFaceDown) ?
                body.StandUpFaceDownBoneTransforms :
                body.StandUpFaceBoneTransforms;

            for (int i = 0; i < body.Bones.Length; i++)
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