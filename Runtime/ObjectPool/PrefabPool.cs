using System.Collections.Generic;

using UnityEngine;

namespace HUtil.Runtime.ObjectPool
{
    /// <summary>
    /// 주어진 프리팹을 이용해 오브젝트를 생성하는 오브젝트 풀링 구현체
    /// </summary>
    /// <typeparam name="T">풀에 저장되는 오브젝트의 타입</typeparam>
    public class PrefabPool<T> : IObjectPool<T> where T : PooledBehaviour<T>
    {
        private readonly GameObject _prefab;
        private readonly Transform _parent;
        private readonly Queue<T> _pooledObjects;
        private readonly List<T> _spawnedObjects;

        /// <summary>
        /// 주어진 프리팹을 이용해 오브젝트를 생성하는 오브젝트 풀링 구현체를 생성합니다
        /// </summary>
        /// <param name="prefab">오브젝트를 생성할 프리팹</param>
        /// <param name="parent">오브젝트를 생성할 부모 트랜스폼</param>
        /// <param name="initialCapacity">초기 생성할 오브젝트 수</param>
        public PrefabPool(GameObject prefab, Transform parent, int initialCapacity = 0)
        {
            _prefab = prefab;
            _parent = parent;
            _pooledObjects = new Queue<T>(initialCapacity);
            _spawnedObjects = new List<T>(initialCapacity);
            for (int i = 0; i < initialCapacity; i++)
            {
                _pooledObjects.Enqueue(CreateNewObject());
            }
        }

        /// <summary>
        /// 주어진 프리팹을 이용해 오브젝트를 생성하는 오브젝트 풀링 구현체를 생성합니다<br>
        /// 동적으로 부모 트랜스폼을 생성합니다
        /// </summary>
        /// <param name="prefab">오브젝트를 생성할 프리팹</param>
        /// <param name="dontDestroyOnLoad">부모 오브젝트를 DontDestroyOnLoad 옵션으로 생성할지 여부</param>
        /// <param name="initialCapacity">초기 생성할 오브젝트 수</param>
        public PrefabPool(T prefab, bool dontDestroyOnLoad = false, int initialCapacity = 0)
        {
            _prefab = prefab.gameObject;
            _parent = new GameObject($"[ObjectPool] {prefab.name}").transform;
            if (dontDestroyOnLoad)
            {
                GameObject.DontDestroyOnLoad(_parent.gameObject);
            }
            _pooledObjects = new Queue<T>(initialCapacity);
            _spawnedObjects = new List<T>(initialCapacity);
            for (int i = 0; i < initialCapacity; i++)
            {
                _pooledObjects.Enqueue(CreateNewObject());
            }
        }

        public T Get()
        {
            T item = GetOrCreateObject();
            _spawnedObjects.Add(item);
            item.OnGetFromPool();
            return item;
        }

        private T GetOrCreateObject()
        {
            if (_pooledObjects.Count > 0)
            {
                return _pooledObjects.Dequeue();
            }
            return CreateNewObject();
        }

        private T CreateNewObject()
        {
            GameObject obj = GameObject.Instantiate(_prefab, _parent);
            T component = obj.GetComponent<T>();
            component.InitializeFromPool(this);
            return component;
        }

        public void Return(T item)
        {
            item.OnReturnToPool();
            _spawnedObjects.Remove(item);
            _pooledObjects.Enqueue(item);
        }

        public void Dispose()
        {
            foreach (T item in _spawnedObjects.ToArray())
            {
                Return(item);
            }
            _spawnedObjects.Clear();
            foreach (T item in _pooledObjects)
            {
                item.CleanupFromPool();
                GameObject.Destroy(item.gameObject);
            }
            _pooledObjects.Clear();
        }
    }
}
