using System;

namespace HUtil.UI
{
    /// <summary>
    /// UI 동기화 방향 플레그
    /// </summary>
    [Flags]
    public enum BindingDirectionFlags
    {
        /// <summary>
        /// 동기화 불가능
        /// </summary>
        None = 0x0,
        /// <summary>
        /// View로 동기화하는 방향
        /// </summary>
        ToUI = 0x1,
        /// <summary>
        /// ViewModel로 역동기화하는 방향
        /// </summary>
        ToData = 0x2,
        /// <summary>
        /// 양방향 전부 가능
        /// </summary>
        Both = 0x3,
    }
}