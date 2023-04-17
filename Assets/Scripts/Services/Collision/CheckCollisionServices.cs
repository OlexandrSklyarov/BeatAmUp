using UnityEngine;

namespace BT
{
    public class CheckCollisionServices
    {
        public readonly Collider[] HitResult;
        
        
        public CheckCollisionServices()
        {
            HitResult = new Collider[10];
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
    }
}