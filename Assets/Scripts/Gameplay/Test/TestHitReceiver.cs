using UnityEngine;

namespace BT
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class TestHitReceiver : MonoBehaviour, IHitReceiver
    {

    }
}