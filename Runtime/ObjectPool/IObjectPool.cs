using UnityEngine;

namespace HUtil.Runtime.ObjectPool
{
    /// <summary>
    /// 오브젝트 풀 인터페이스
    /// </summary>
    /// <typeparam name="T">풀에 저장되는 오브젝트의 타입</typeparam>
    public interface IObjectPool<T>
    {
        /// <summary>
        /// 풀에서 오브젝트를 꺼냅니다
        /// </summary>
        /// <returns>풀에서 꺼낸 오브젝트</returns>
        public T Get();

        /// <summary>
        /// 오브젝트를 풀에 반환합니다
        /// </summary>
        /// <param name="item">풀에 반환할 오브젝트</param>
        public void Return(T item);

        /// <summary>
        /// 풀에서 생성된 모든 오브젝트를 제거하고 풀을 삭제합니다
        /// </summary>
        public void Dispose();
    }
}
