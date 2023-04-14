using UnityEngine;

namespace BT
{
    public class HeroViewProvider : MonoBehaviour, IHitReceiver
    {        
        [field: SerializeField] public Transform CameraLookPoint {get; private set;}
        [field: SerializeField] public Transform CameraFollowPoint {get; private set;}
        [field: SerializeField] public Transform BodyHips {get; private set;}
        
        public void AddForceDamage(Vector3 force)
        {
            Util.Debug.Print($"hero add damage force... {force}");
        }
    }
}