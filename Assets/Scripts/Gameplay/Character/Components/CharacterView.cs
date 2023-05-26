using UnityEngine;

namespace BT
{
    public struct CharacterView
    {
        public Animator Animator;
        public Transform ViewTransform;        
        public Transform HipBone;
        public BodyMaterialProvider BodyMaterials;
        public float Height;
        public float BodyRadius;
    }
}

