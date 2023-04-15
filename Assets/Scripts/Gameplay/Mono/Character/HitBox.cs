using UnityEngine;

namespace BT
{
    [RequireComponent(typeof(Rigidbody))]
    public class HitBox : MonoBehaviour
    {
        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic = true;
        }
    }
}