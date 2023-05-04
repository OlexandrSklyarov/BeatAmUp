using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public struct CharacterPhysicsBody
    {
        public CapsuleCollider Collider;
        public IEnumerable<Rigidbody> BodyRagdoll;
        public BoneTransform[] RagdollBoneTransforms;
        public BoneTransform[] StandUpFaceBoneTransforms;
        public BoneTransform[] StandUpFaceDownBoneTransforms;
        public Transform[] Bones;
    }
}