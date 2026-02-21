using System.Collections.Generic;

using UnityEngine;

namespace HUtil.Runtime.ObjectPool
{
    /// <summary>
    /// 런타임에 바인딩되는 프리팹을 이용해 오브젝트를 생성하는 오브젝트 풀링 구현체
    /// </summary>
    /// <typeparam name="T">풀에 저장되는 오브젝트의 타입</typeparam>
    public class RuntimePrefabPool<T> : MonoBehaviour, IObjectPool<T> where T : PooledBehaviour<T>
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private Transform _parent;
        [SerializeField] private int _initialCapacity = 0;
        private Queue<T> _pooledObjects;
        private List<T> _spawnedObjects;

        private void Awake(){
            _pooledObjects = new Queue<T>();
            _spawnedObjects = new List<T>();
            for (int i = 0; i < _initialCapacity; i++)
            {
                _pooledObjects.Enqueue(CreateNewObject());
            }
        }

        private void OnDestroy(){
            Dispose();
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
