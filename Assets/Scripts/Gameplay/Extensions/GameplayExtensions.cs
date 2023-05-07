using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public static class GameplayExtensions
    {
        public static void CreateVfxEntity(EcsWorld world, EcsPool<VfxView> pool, 
            SharedData data, Vector3 position, VfxType type)
        {
            var entity = world.NewEntity();
            ref var vfx = ref pool.Add(entity);
            
            var view = data.VFXController.PlayHitVFX(type, position);
            vfx.View = view;
            vfx.LifeTime = view.LifeTime;   
        }


        public static bool IsCurrentAnimationState(this Animator animator, string animationState, int layer = 0)
        {
            return animator.GetCurrentAnimatorStateInfo(layer).IsName(animationState);
        }


        public static void PopulateBoneTransforms(Transform[] bones, BoneTransform[] boneTransforms)
        {
            for(int i = 0; i < bones.Length; i++)
            {
                boneTransforms[i].Position = bones[i].localPosition;
                boneTransforms[i].Rotation = bones[i].localRotation;
            }
        }
    }
}