using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public struct CharacterPhysicsBody
    {
        public IEnumerable<Rigidbody> BodyRagdoll;
        public CapsuleCollider Collider;
    }
}