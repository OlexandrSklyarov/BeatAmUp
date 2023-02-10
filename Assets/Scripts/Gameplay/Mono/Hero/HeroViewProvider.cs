using UnityEngine;

namespace BT
{
    public class HeroViewProvider : MonoBehaviour, IHitReceiver
    {
        public string MyName => name;

        
        [field: SerializeField] public Transform CameraLookPoint {get; private set;}
        [field: SerializeField] public Transform CameraFollowPoint {get; private set;}


        void IHitReceiver.Hit()
        {
        }
    }
}