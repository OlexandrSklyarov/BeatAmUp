using UnityEngine;

namespace BT
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(Animator))]
    public class TestHitReceiver : MonoBehaviour, IHitReceiver
    {       
        
    }
}