using UnityEngine;

namespace BT
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(Animator))]
    public class TestHitReceiver : MonoBehaviour, IHitReceiver
    {        
        public string MyName => name;

        
        void IHitReceiver.Hit()
        {
            GetComponent<Animator>().CrossFade("HIT", 0f, 0);
        }
    }
}