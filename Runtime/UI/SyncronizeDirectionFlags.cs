using System;

namespace HUtil.Runtime.UI
{
    /// <summary>
    /// UI 동기화 방향 플레그
    /// </summary>
    [Flags]
    public enum SyncronizeDirectionFlags
    {
        /// <summary>
        /// View로 동기화하는 방향
        /// </summary>
        ToTarget = 0x1,
        /// <summary>
        /// ViewModel로 역동기화하는 방향
        /// </summary>
        ToSource = 0x2,
        /// <summary>
        /// 양방향 전부 가능
        /// </summary>
        Both = 0x3,
    }
}