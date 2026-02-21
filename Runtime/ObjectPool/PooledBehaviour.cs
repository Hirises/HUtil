using System;

using UnityEngine;

namespace HUtil.Runtime.ObjectPool
{
    /// <summary>
    /// 오브젝트 풀에서 생성/관리되는 MonoBehaviour
    /// </summary>
    /// <typeparam name="T">PooledBehaviour의 타입</typeparam>
    public abstract class PooledBehaviour<T> : MonoBehaviour where T : PooledBehaviour<T>
    {
        private IObjectPool<T> _pool;

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
            OnDestroyFromPool();
            _pool = null;
        }

        /// <summary>
        /// 오브젝트가 풀에서 생성될 때 호출됩니다
        /// </summary>
        protected virtual void OnCreateFromPool(){}

        /// <summary>
        /// 오브젝트가 풀에서 꺼내질 때 호출됩니다
        /// </summary>
        internal virtual void OnGetFromPool(){
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 오브젝트가 풀에 반환될 때 호출됩니다
        /// </summary>
        internal virtual void OnReturnToPool(){
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 오브젝트가 풀에서 삭제될 때 호출됩니다
        /// </summary>
        protected virtual void OnDestroyFromPool(){}

        /// <summary>
        /// 이 오브젝트를 풀에 반환합니다
        /// </summary>
        /// <exception cref="InvalidOperationException">풀이 설정되지 않은 경우</exception>
        public void ReturnSelf()
        {
            if (_pool == null){
                throw new InvalidOperationException("Pool is not set");
            }
            _pool.Return(this as T);
        }
    }
}
