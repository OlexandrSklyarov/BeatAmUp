using System.Collections.Generic;
using Services.Pooling;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    [SelectionBase]
    [RequireComponent(typeof(NavMeshAgent), typeof(CapsuleCollider))]
    public class EnemyViewProvider : MonoBehaviour, IPoolable
    {
        [field: SerializeField] public Transform BodyHips {get; private set;}
        public IEnumerable<Rigidbody> RagdollElements => _ragdollElements;
        public CapsuleCollider Collider => _collider;

        private IFactoryStorage<EnemyViewProvider> _storage;
        private CapsuleCollider _collider;
        private Rigidbody[] _ragdollElements;


        public void Init(IFactoryStorage<EnemyViewProvider> storage)
        {
            _storage = storage;

            _collider = GetComponent<CapsuleCollider>();

            _ragdollElements ??= GetRagdollElements();
        }
        
        
        void IPoolable.ReturnToStorage() => _storage?.ReturnToStorage(this);


        private Rigidbody[] GetRagdollElements()
        {
            var rbCollection = GetComponentsInChildren<Rigidbody>();
            return rbCollection;
        }
    }
}