using System;
using UnityEngine;

namespace BT
{
    public class CheckCollisionServices
    {
        public readonly Collider[] HitResult;
        
        
        public CheckCollisionServices()
        {
            HitResult = new Collider[20];
        }
        
        
        public int CheckHurtBox(HurtBox hurtBox, LayerMask layerMask)
        {
            return Physics.OverlapBoxNonAlloc
            (
                hurtBox.Position,
                hurtBox.HalfExtend,
                HitResult,
                Quaternion.identity,
                layerMask
            );
        }


        public int OverlapSphere(Transform origin, float radius, LayerMask layerMask)
        {
            return Physics.OverlapSphereNonAlloc
            (
                origin.position,
                radius,
                HitResult,
                layerMask
            );
        }
    }
}