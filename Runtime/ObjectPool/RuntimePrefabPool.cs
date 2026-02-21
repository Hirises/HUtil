using System;
using System.Collections.Generic;

using UnityEngine;

namespace HUtil.Runtime.ObjectPool
{
    /// <summary>
    /// 런타임에 바인딩되는 프리팹을 이용해 오브젝트를 생성하는 오브젝트 풀링 구현체 (비제네릭)<br/>
    /// 인스펙터에서 직접 컴포넌트로 부착하여 사용할 수 있습니다
    /// </summary>
    public class RuntimePrefabPool : MonoBehaviour, IObjectPool<IPooledBehaviour>
    {
        [SerializeField] protected GameObject _prefab;
        [SerializeField] protected Transform _parent;
        [SerializeField] protected int _initialCapacity = 0;

        private Queue<IPooledBehaviour> _pooledObjects;
        private List<IPooledBehaviour> _spawnedObjects;

        protected virtual void Awake()
        {
            _pooledObjects = new Queue<IPooledBehaviour>();
            _spawnedObjects = new List<IPooledBehaviour>();
            for (int i = 0; i < _initialCapacity; i++)
            {
                _pooledObjects.Enqueue(CreateNewObject());
            }
        }

        protected virtual void OnDestroy()
        {
            Dispose();
        }

        /// <summary>
        /// 풀에서 오브젝트를 꺼냅니다
        /// </summary>
        public IPooledBehaviour Get()
        {
            IPooledBehaviour item = GetOrCreateObject();
            _spawnedObjects.Add(item);
            item.OnGetFromPool();
            return item;
        }

        /// <summary>
        /// 풀에서 오브젝트를 꺼내 지정된 타입으로 반환합니다
        /// </summary>
        /// <typeparam name="T">반환할 타입</typeparam>
        /// <exception cref="InvalidCastException">타입이 일치하지 않는 경우</exception>
        public T GetAs<T>() where T : class, IPooledBehaviour
        {
            IPooledBehaviour item = Get();
            if (item is T typed)
            {
                return typed;
            }
            throw new InvalidCastException($"Pool contains {item.GetType()}, not {typeof(T)}");
        }

        private IPooledBehaviour GetOrCreateObject()
        {
            if (_pooledObjects.Count > 0)
            {
                return _pooledObjects.Dequeue();
            }
            return CreateNewObject();
        }

        protected virtual IPooledBehaviour CreateNewObject()
        {
            GameObject obj = Instantiate(_prefab, _parent);
            IPooledBehaviour component = obj.GetComponent<IPooledBehaviour>();
            if (component == null)
            {
                throw new InvalidOperationException($"Prefab '{_prefab.name}' does not have a component implementing IPooledBehaviour");
            }
            component.InitializeFromPool();
            return component;
        }

        /// <summary>
        /// 오브젝트를 풀에 반환합니다
        /// </summary>
        public void Return(IPooledBehaviour item)
        {
            item.OnReturnToPool();
            _spawnedObjects.Remove(item);
            _pooledObjects.Enqueue(item);
        }

        /// <summary>
        /// 오브젝트를 풀에 반환합니다
        /// </summary>
        public void Return<T>(T item) where T : class, IPooledBehaviour
        {
            Return((IPooledBehaviour)item);
        }

        /// <summary>
        /// 풀에서 생성된 모든 오브젝트를 제거하고 풀을 삭제합니다
        /// </summary>
        public void Dispose()
        {
            if (_spawnedObjects == null) return;

            foreach (IPooledBehaviour item in _spawnedObjects.ToArray())
            {
                Return(item);
            }
            _spawnedObjects.Clear();
            foreach (IPooledBehaviour item in _pooledObjects)
            {
                item.CleanupFromPool();
                if (item is MonoBehaviour mb)
                {
                    Destroy(mb.gameObject);
                }
            }
            _pooledObjects.Clear();
        }
    }

    /// <summary>
    /// 런타임에 바인딩되는 프리팹을 이용해 오브젝트를 생성하는 오브젝트 풀링 구현체 (제네릭)<br/>
    /// 타입 안전성이 필요한 경우 이 클래스를 상속하여 사용합니다<br/>
    /// <example>
    /// <code>public class BulletPool : RuntimePrefabPool&lt;Bullet&gt; { }</code>
    /// </example>
    /// </summary>
    /// <typeparam name="T">풀에 저장되는 오브젝트의 타입</typeparam>
    public class RuntimePrefabPool<T> : RuntimePrefabPool, IObjectPool<T> where T : PooledBehaviour<T>
    {
        private Queue<T> _pooledObjectsTyped;
        private List<T> _spawnedObjectsTyped;

        protected override void Awake()
        {
            _pooledObjectsTyped = new Queue<T>();
            _spawnedObjectsTyped = new List<T>();
            for (int i = 0; i < _initialCapacity; i++)
            {
                _pooledObjectsTyped.Enqueue(CreateNewObjectTyped());
            }
        }

        protected override void OnDestroy()
        {
            DisposeTyped();
        }

        /// <summary>
        /// 풀에서 오브젝트를 꺼냅니다
        /// </summary>
        public new T Get()
        {
            T item = GetOrCreateObjectTyped();
            _spawnedObjectsTyped.Add(item);
            item.OnGetFromPool();
            return item;
        }

        private T GetOrCreateObjectTyped()
        {
            if (_pooledObjectsTyped.Count > 0)
            {
                return _pooledObjectsTyped.Dequeue();
            }
            return CreateNewObjectTyped();
        }

        private T CreateNewObjectTyped()
        {
            GameObject obj = Instantiate(_prefab, _parent != null ? _parent : transform);
            T component = obj.GetComponent<T>();
            if (component == null)
            {
                throw new InvalidOperationException($"Prefab '{_prefab.name}' does not have component of type {typeof(T)}");
            }
            component.InitializeFromPool(this);
            return component;
        }

        /// <summary>
        /// 오브젝트를 풀에 반환합니다
        /// </summary>
        public void Return(T item)
        {
            item.OnReturnToPool();
            _spawnedObjectsTyped.Remove(item);
            _pooledObjectsTyped.Enqueue(item);
        }

        private void DisposeTyped()
        {
            if (_spawnedObjectsTyped == null) return;

            foreach (T item in _spawnedObjectsTyped.ToArray())
            {
                Return(item);
            }
            _spawnedObjectsTyped.Clear();
            foreach (T item in _pooledObjectsTyped)
            {
                item.CleanupFromPool();
                Destroy(item.gameObject);
            }
            _pooledObjectsTyped.Clear();
        }

        /// <summary>
        /// 풀에서 생성된 모든 오브젝트를 제거하고 풀을 삭제합니다
        /// </summary>
        public new void Dispose()
        {
            DisposeTyped();
        }
    }
}
