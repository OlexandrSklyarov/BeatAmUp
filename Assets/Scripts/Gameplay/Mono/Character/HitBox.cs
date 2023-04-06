using UnityEngine;

namespace BT
{
    [RequireComponent(typeof(Rigidbody))]
    public class HitBox : MonoBehaviour, IHitReceiver
    {
        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic = true;
            
            gameObject.layer = LayerMask.NameToLayer(ConstPrm.Character.HIT_LAYER_NAME);
        }


        private void OnValidate()
        {
            gameObject.layer = LayerMask.NameToLayer(ConstPrm.Character.HIT_LAYER_NAME);
        }


        public void AddForceDamage(Vector3 force)
        {
            _rb.AddForce(force, ForceMode.Impulse);
        }
    }
}