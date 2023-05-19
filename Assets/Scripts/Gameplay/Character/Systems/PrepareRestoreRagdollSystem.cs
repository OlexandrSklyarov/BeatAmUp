using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public class PrepareRestoreRagdollSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
                
            var enemies = world
                .Filter<RagdollState>()
                .Inc<CharacterPhysicsBody>()
                .Inc<CharacterView>()
                .Inc<MovementAI>()
                .Exc<RestoreRagdollState>()
                .Exc<Stun>()
                .Exc<Death>()
                .End();

            var restoreRagdollPool = world.GetPool<RestoreRagdollState>();
            var bodyPool = world.GetPool<CharacterPhysicsBody>();
            var viewPool = world.GetPool<CharacterView>();
            var aiPool = world.GetPool<MovementAI>();

            foreach (var ent in enemies)
            {
                ref var body = ref bodyPool.Get(ent);
                ref var view = ref viewPool.Get(ent);
                ref var ai = ref aiPool.Get(ent);               

                var isFaceDown = IsFaceDown(ref view);
                var isCanStandUp = IsCanStandUp(ref view, ref ai);

                ref var comp = ref restoreRagdollPool.Add(ent);

                if (!isCanStandUp) continue;

                comp.IsCanStandUp = isCanStandUp;
                comp.IsFaceDown = isFaceDown;
                
                PrepareBonesToRestore(ref body, ref view, ref comp);
                ResetRagdoll(ref body);
            }
        }


        private bool IsFaceDown(ref CharacterView view)
        {
            var dot = Vector3.Dot(view.HipBone.forward, Vector3.up);
            return dot < 0f;
        }


        private bool IsCanStandUp(ref CharacterView view, ref MovementAI ai)
        {            
            var origin = view.HipBone.position;
            var isStandSuccess = ai.NavAgent.Warp(origin);            

            if (isStandSuccess) 
            {                   
                view.HipBone.position = new Vector3(origin.x, view.HipBone.position.y, origin.z);
            }

            return isStandSuccess;
        }
        

        private void PrepareBonesToRestore(ref CharacterPhysicsBody body, ref CharacterView view, 
            ref RestoreRagdollState comp)
        {
            var originalHipsPos = view.HipBone.transform.position;
            var originalHipsRot = view.HipBone.transform.rotation;

            var side  = (comp.IsFaceDown) ? 1f : -1f;
            var desiredDirection = view.HipBone.transform.up * side;
            desiredDirection.y = 0f;
            desiredDirection.Normalize();

            view.ViewTransform.rotation = Util.Vector3Math.DirToQuaternion(desiredDirection);

            view.HipBone.transform.position = originalHipsPos;
            view.HipBone.transform.rotation = originalHipsRot; 

            GameplayExtensions.PopulateBoneTransforms(body.Bones, body.RagdollBoneTransforms);           
        }

        
        private void ResetRagdoll(ref CharacterPhysicsBody body)
        {
            foreach (var rb in body.BodyRagdoll) rb.isKinematic = true;
        } 
    }
}