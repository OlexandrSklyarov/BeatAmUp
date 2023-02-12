using Services.Pooling;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    [SelectionBase]
    [RequireComponent(typeof(NavMeshAgent), typeof(CapsuleCollider), typeof(Rigidbody))]
    public class EnemyViewProvider : MonoBehaviour, IHitReceiver, IFactoryStorage<EnemyViewProvider>
    {
        private IFactoryStorage<EnemyViewProvider> _storage;
   

        public void Init(IFactoryStorage<EnemyViewProvider> storage)
        {
            _storage = storage;
        }
        
        
        void IFactoryStorage<EnemyViewProvider>.ReturnToStorage(EnemyViewProvider item)
        {
            _storage?.ReturnToStorage(item);
        }
    }
}