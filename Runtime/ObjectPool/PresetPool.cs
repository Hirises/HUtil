using System.Collections.Generic;

using UnityEngine;

namespace HUtil.Runtime.ObjectPool
{
    /// <summary>
    /// 기존 오브젝트를 복사해 오브젝트를 생성하는 오브젝트 풀링 구현체
    /// </summary>
    /// <typeparam name="T">풀에 저장되는 오브젝트의 타입</typeparam>
    public class PresetPool<T> : IObjectPool<T> where T : PooledBehaviour<T>
    {
        private readonly T _preset;
        private readonly Transform _parent;
        private readonly Queue<T> _pooledObjects;
        private readonly List<T> _spawnedObjects;

        /// <summary>
        /// 기존 오브젝트를 복사해 오브젝트를 생성하는 오브젝트 풀링 구현체를 생성합니다
        /// </summary>
        /// <param name="preset">오브젝트를 생성할 프리팹</param>
        /// <param name="parent">오브젝트를 생성할 부모 트랜스폼, 기본값은 프리셋의 부모 트랜스폼</param>
        /// <param name="initialCapacity">초기 생성할 오브젝트 수</param>
        public PresetPool(T preset, Transform parent = null, int initialCapacity = 0)
        {
            _preset = preset;
            if (parent == null){
                parent = preset.transform.parent;
            }
            _parent = parent;
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
            GameObject obj = GameObject.Instantiate(_preset.gameObject, _parent);
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
