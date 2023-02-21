using Services.Pooling;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    [SelectionBase]
    [RequireComponent(typeof(NavMeshAgent), typeof(CapsuleCollider), typeof(Rigidbody))]
    public class EnemyViewProvider : MonoBehaviour, IHitReceiver
    {
        private IFactoryStorage<EnemyViewProvider> _storage;


        public void Init(IFactoryStorage<EnemyViewProvider> storage)
        {
            _storage = storage;
        }
        
        
        public void ReturnToStorage() => _storage?.ReturnToStorage(this);        
    }
}