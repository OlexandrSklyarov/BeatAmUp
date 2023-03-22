using UnityEngine;

namespace Util
{
    public static class AnimationExtensions
    {
        public static float GetLengthState(this Animator animator, string stateName)
        {
            var clips = animator.runtimeAnimatorController.animationClips;
     
            for(int i = 0; i < clips.Length; i++) 
            {
                if(clips[i].name == stateName) return clips[i].length;
            }

            return 0f;
        }
    }
}