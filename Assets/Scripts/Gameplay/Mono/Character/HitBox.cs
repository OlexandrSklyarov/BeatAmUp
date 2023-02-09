using UnityEngine;

namespace BT
{
    [RequireComponent(typeof(SphereCollider))]
    public class HitBox : MonoBehaviour
    {
        public SphereCollider Collider => _collider;
        public HitType Type => _type;

        [SerializeField] private HitType _type;

        private SphereCollider _collider;


        public void Init()
        {
            _collider = GetComponent<SphereCollider>();
            _collider.isTrigger = true;
        }
    }
}