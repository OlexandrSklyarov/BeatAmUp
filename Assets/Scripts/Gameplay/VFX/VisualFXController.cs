using BT;
using Gameplay.Factories;
using Services.Pooling;
using UnityEngine;

namespace Gameplay.FX
{
    public class VisualFXController : IFactoryStorage<BaseVFXItem>
    {
        private readonly VfxFactory _factory;


        public VisualFXController(VfxData config)
        {            
            _factory = new VfxFactory(config);          
        }     


        public BaseVFXItem PlayHitVFX(VfxType type, Vector3 position)
        {
            var vfx = _factory.GetVFX(type);
            var item =  PlayVfx(vfx, position, Quaternion.identity);
            
            return item;
        }
        

        private BaseVFXItem PlayVfx(BaseVFXItem vfx, Vector3 position, Quaternion rotation)
        {
            vfx.Init(this);
            vfx.transform.SetPositionAndRotation(position, rotation);
            vfx.Play();

            return vfx;
        }      
        
       
        void IFactoryStorage<BaseVFXItem>.ReturnToStorage(BaseVFXItem item)
        {
            if (item is GameWorldVFX)_factory.ReturnToStorage(item as GameWorldVFX);
        }
    }
}