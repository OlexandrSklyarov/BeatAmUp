using UnityEngine;

namespace Services.Pooling
{
    public interface IFactoryStorage<T> where T : MonoBehaviour
    {
        void ReturnToStorage(T item);
    }
}