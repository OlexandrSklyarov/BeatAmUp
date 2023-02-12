using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Services.Pooling
{
    public class Pool<T> where T : MonoBehaviour
    {
        #region Data
        private class PoolElement
        {
            public T RealGO;
            public bool Used;

            public PoolElement(T GO)
            {
                RealGO = GO;
                Used = false;
            }
        }

        #endregion

        private readonly Transform _container;
        private readonly GameObject _template;
        private readonly int _startSize;
        private readonly int _additionalSize;
        private readonly List<PoolElement> _pool;


        public Pool(GameObject template, Transform container, int startSize)
        {
            if (template == null)
            {
                Debug.LogError($"Template is NULL");
                return;
            }

            if (template.GetComponent<T>() == null)
            {
                Debug.LogError($"Template must contain {typeof(T)} component.");
                return;
            }

            _template = template;
            _container = container;
            _startSize = Mathf.Max(startSize, 1);
            _additionalSize = _startSize;

            _pool = new List<PoolElement>();

            SpawnElements(_startSize);
        }


        public T GetElement()
        {
            var element = _pool.FirstOrDefault(element => element.Used == false);

            if (element == null)
            {
                Debug.Log("pool empty => create new instance!!!");
                SpawnElements(_additionalSize);
                return GetElement();
            }

            element.Used = true;
            element.RealGO.gameObject.SetActive(true);
            return element.RealGO;
        }


        public void ReturnToPool(T element)
        {
            var poolElement = _pool.FirstOrDefault(e => e.RealGO == element);

            if (poolElement == null)
            {
                Debug.LogWarning("Object is not element of pool.");
                return;
            }

            poolElement.RealGO.gameObject.SetActive(false);
            poolElement.RealGO.transform.SetParent(_container);
            poolElement.Used = false;

            CheckPoolSize();
        }


        private void SpawnElements(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var go = Object.Instantiate(_template, _container) as GameObject;
                go.SetActive(false);
                _pool.Add(new PoolElement(go.GetComponent<T>()));
            }
        }


        private void CheckPoolSize()
        {
            if (_pool.Count <= _startSize) return;

            var freePoolElements = _pool.FindAll(e => e.Used == false).ToArray();

            if (freePoolElements.Length <= _additionalSize) return;

            for (int i = 0; i < _additionalSize; i++)
            {
                var el = freePoolElements[i];
                _pool.Remove(el);
                Object.Destroy(el.RealGO.gameObject);
            }

            _pool.Capacity = _pool.Count;
        }
    }
}