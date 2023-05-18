using System.Collections.Generic;
using Services.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    [SelectionBase]
    [RequireComponent(typeof(NavMeshAgent), typeof(CapsuleCollider))]
    public class EnemyViewProvider : MonoBehaviour, IPoolable
    {
        [field: SerializeField] public Transform BodyHips {get; private set;}
        [field: SerializeField] public IEnumerable<HitBox> HitBoxes { get; private set; }
        [field: SerializeField] public IEnumerable<HurtBox> HurtBoxes { get; private set; }
        [field: SerializeField] public Transform BodyView { get; private set; }
        public IEnumerable<Rigidbody> RagdollElements => _ragdollElements;
        public CapsuleCollider Collider => _collider;

        public Animator Animator => _animator;

        private IFactoryStorage<EnemyViewProvider> _storage;
        private Animator _animator;
        private CapsuleCollider _collider;
        private Rigidbody[] _ragdollElements;
        private bool _isInit;

        public void Init(IFactoryStorage<EnemyViewProvider> storage)
        {
            _storage = storage;

            if (!_isInit)
            {
                HitBoxes = GetComponentsInChildren<HitBox>();
                HurtBoxes = GetComponentsInChildren<HurtBox>();
                
                foreach(var h in HurtBoxes) h.Init();

                _animator = GetComponentInChildren<Animator>();
                _collider = GetComponent<CapsuleCollider>();
                _ragdollElements ??= GetRagdollElements();
                _isInit = true;
            }
        }
        
        
        void IPoolable.ReturnToStorage() => _storage?.ReturnToStorage(this);


        private Rigidbody[] GetRagdollElements()
        {
            var rbCollection = GetComponentsInChildren<Rigidbody>();
            return rbCollection;
        }
    }
}