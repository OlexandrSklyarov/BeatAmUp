using Services.Pooling;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    [SelectionBase]
    [RequireComponent(typeof(NavMeshAgent), typeof(CapsuleCollider), typeof(Rigidbody))]
    public class EnemyViewProvider : MonoBehaviour, IHitReceiver
    {
        public Vector3 BodyHipsPosition => _modelHips.position;

        [SerializeField] private Transform _modelHips;

        private IFactoryStorage<EnemyViewProvider> _storage;


        public void Init(IFactoryStorage<EnemyViewProvider> storage) => _storage = storage;
        
        
        public void ReturnToStorage() => _storage?.ReturnToStorage(this);       
    }
}