using UnityEngine;
using UnityEngine.Pool;

namespace _Project.Scripts.Utilities.Pool
{
    public class AbstractPool<T> where T : Component, IPoolable
    {
        private readonly ObjectPool<T> _pool;
        private readonly T _prefab;
        private readonly Transform _container;

        public AbstractPool(T prefab, Transform container, bool collectionCheck, int defaultCapacity, int maxSize)
        {
            _prefab = prefab;
            _container = container;

            _pool = new ObjectPool<T>(
                createFunc: Create,
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: OnDestroy,
                collectionCheck: collectionCheck,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize);
        }

        public T Get() => _pool.Get();

        public void Release(T instance) => _pool.Release(instance);

        public void Clear() => _pool.Clear();

        private T Create()
        {
            var instance = Object.Instantiate(_prefab, _container);
            instance.gameObject.SetActive(false);
            return instance;
        }

        private static void OnGet(T instance)
        {
            instance.gameObject.SetActive(true);
        }

        private static void OnRelease(T instance)
        {
            instance.gameObject.SetActive(false);
        }

        private static void OnDestroy(T instance)
        {
            if (instance != null)
            {
                Object.Destroy(instance.gameObject);
            }
        }
    }
}
