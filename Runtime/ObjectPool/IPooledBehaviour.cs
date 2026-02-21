namespace HUtil.Runtime.ObjectPool
{
    /// <summary>
    /// 오브젝트 풀에서 관리되는 오브젝트의 비제네릭 인터페이스
    /// </summary>
    public interface IPooledBehaviour
    {
        /// <summary>
        /// 오브젝트가 풀에서 생성될 때 호출됩니다
        /// </summary>
        internal void InitializeFromPool();

        /// <summary>
        /// 오브젝트가 풀에서 삭제될 때 호출됩니다
        /// </summary>
        internal void CleanupFromPool();

        /// <summary>
        /// 오브젝트가 풀에서 꺼내질 때 호출됩니다
        /// </summary>
        internal void OnGetFromPool();

        /// <summary>
        /// 오브젝트가 풀에 반환될 때 호출됩니다
        /// </summary>
        internal void OnReturnToPool();
    }
}
