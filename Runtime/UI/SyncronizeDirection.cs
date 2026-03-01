using System;

namespace HUtil.Runtime.UI
{
    /// <summary>
    /// 바인딩 동기화 방식
    /// </summary>
    public enum SyncronizeDirection
    {
        /// <summary>
        /// 동기화하지 않음
        /// </summary>
        None,

        /// <summary>
        /// 대상 UI에 1회만 동기화
        /// </summary>
        OneShotToTarget,
        /// <summary>
        /// 대상 UI로만 동기화
        /// </summary>
        OneWayToTarget,
        /// <summary>
        /// 대상 ViewModel로만 동기화
        /// </summary>
        OneWayToSource,
        /// <summary>
        /// 양방향 동기화
        /// </summary>
        TwoWay,
    }
}