using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Services.AddressablesProvider
{
    public abstract class BaseLocalAssetProvider
    {
        private GameObject _cashObject;


        protected async Task<T> LoadInternal<T>(string assetId)
        {
            var handle = Addressables.InstantiateAsync(assetId);
            
            _cashObject = await handle.Task;

            if (!_cashObject.TryGetComponent(out T component))
            {
                throw new NullReferenceException($"Object of type {typeof(T)} is null. Attempt to load it from Addressables.");
            }

            return component;
        }


        protected void UploadInternal()
        {
            if (_cashObject == null) return;

            _cashObject.SetActive(false);
            Addressables.ReleaseInstance(_cashObject);
            _cashObject = null;
        }
    }
}