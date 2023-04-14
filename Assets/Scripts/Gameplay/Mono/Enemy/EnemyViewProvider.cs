using System.Collections.Generic;
using Services.Pooling;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    [SelectionBase]
    [RequireComponent(typeof(NavMeshAgent), typeof(CapsuleCollider))]
    public class EnemyViewProvider : MonoBehaviour
    {
        [field: SerializeField] public Transform BodyHips {get; private set;}
        public IEnumerable<Rigidbody> RagdollElements => _ragdollElements;

        private IFactoryStorage<EnemyViewProvider> _storage;
        private Rigidbody[] _ragdollElements;


        public void Init(IFactoryStorage<EnemyViewProvider> storage)
        {
            _storage = storage;

            _ragdollElements ??= GetRagdollElements();
        }
        
        
        public void ReturnToStorage() => _storage?.ReturnToStorage(this);


        private Rigidbody[] GetRagdollElements()
        {
            var rbCollection = GetComponentsInChildren<Rigidbody>();
            return rbCollection;
        }
    }
}