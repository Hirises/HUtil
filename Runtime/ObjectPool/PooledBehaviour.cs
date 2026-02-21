using System;

using UnityEngine;

namespace HUtil.Runtime.ObjectPool
{
    /// <summary>
    /// 오브젝트 풀에서 생성/관리되는 MonoBehaviour
    /// </summary>
    /// <typeparam name="T">PooledBehaviour의 타입</typeparam>
    public abstract class PooledBehaviour<T> : MonoBehaviour, IPooledBehaviour where T : PooledBehaviour<T>
    {
        private IObjectPool<T> _pool;

        #region IPooledBehaviour Implementation
        /// <summary>
        /// 비제네릭 풀에서 호출되는 초기화 메서드
        /// </summary>
        void IPooledBehaviour.InitializeFromPool()
        {
            OnCreateFromPool();
        }

        /// <summary>
        /// 이 오브젝트의 삭제 전 처리를 진행합니다
        /// </summary>
        void IPooledBehaviour.CleanupFromPool()
        {
            CleanupFromPool();
        }

        /// <summary>
        /// 오브젝트가 풊에서 꺼내질 때 호출됩니다
        /// </summary>
        void IPooledBehaviour.OnGetFromPool()
        {
            OnGetFromPool();
        }

        /// <summary>
        /// 오브젝트가 풀에 반환될 때 호출됩니다
        /// </summary>
        void IPooledBehaviour.OnReturnToPool()
        {
            OnReturnToPool();
        }
        #endregion

        /// <summary>
        /// 주어진 풀 정보를 이용해 해당 오브젝트를 초기화 처리를 진행합니다
        /// </summary>
        /// <param name="pool">이 오브젝트가 생성된 풀</param>
        internal void InitializeFromPool(IObjectPool<T> pool)
        {
            _pool = pool;
            OnCreateFromPool();
        }

        /// <summary>
        /// 이 오브젝트의 삭제 전 처리를 진행합니다
        /// </summary>
        internal void CleanupFromPool()
        {
            OnCleanupFromPool();
            _pool = null;
        }

        /// <summary>
        /// 오브젝트가 풀에서 생성될 때 호출됩니다
        /// </summary>
        protected virtual void OnCreateFromPool()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 오브젝트가 풀에서 꺼내질 때 호출됩니다
        /// </summary>
        internal virtual void OnGetFromPool()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 오브젝트가 풀에 반환될 때 호출됩니다
        /// </summary>
        internal virtual void OnReturnToPool()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 오브젝트가 풀에서 삭제될 때 호출됩니다
        /// </summary>
        protected virtual void OnCleanupFromPool() { }

        /// <summary>
        /// 이 오브젝트를 풀에 반환합니다
        /// </summary>
        /// <exception cref="InvalidOperationException">풀이 설정되지 않은 경우</exception>
        public void ReturnSelf()
        {
            if (_pool == null)
            {
                throw new InvalidOperationException("Pool is not set. This object may have been created from a non-generic pool.");
            }
            _pool.Return(this as T);
        }
    }
}
