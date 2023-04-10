using UnityEngine;

namespace BT
{
    [RequireComponent(typeof(BoxCollider))]
    public class HurtBox : MonoBehaviour
    {
        public Vector3 Position => _collider.transform.position;
        public Vector3 HalfExtend => _collider.bounds.extents * 0.5f;
        public HitType Type => _type;

        [SerializeField] private HitType _type;

        private BoxCollider _collider;


        public void Init()
        {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }
    }
}