using Services.Pooling;
using UnityEngine;

namespace BT
{
    public abstract class BaseVFXItem : MonoBehaviour
    {
        public float LifeTime => _lifeTime;
        public bool IsInfinity => _isInfinity;

        [SerializeField] private bool _isInfinity;
        [SerializeField, Min(0f)] private float _lifeTime;
        [SerializeField] private ParticleSystem _vfx;

        private IFactoryStorage<BaseVFXItem> _storage;
        
        
        public void Init(IFactoryStorage<BaseVFXItem> storage) => _storage = storage;


        public void Play() => _vfx.Play();  
        

        public void Restore()
        {            
            _vfx.Stop();
            _storage?.ReturnToStorage(this);
        }
    }
}