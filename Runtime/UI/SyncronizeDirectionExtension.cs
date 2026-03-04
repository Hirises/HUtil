using System;

namespace HUtil.Runtime.UI
{
    /// <summary>
    /// 동기화 방향 확장 메서드
    /// </summary>
    public static class SyncronizeDirectionExtension
    {
        /// <summary>
        /// 입력된 동기화 방향이 허용되는지 여부를 반환합니다
        /// </summary>
        /// <param name="flags">허용되는 동기화 방향</param>
        /// <param name="direction">확인할 동기화 방향</param>
        /// <returns>혀용여부</returns>
        public static bool IsAllowed(this SyncronizeDirectionFlags flags, SyncronizeDirection direction){
            switch(direction){
                case SyncronizeDirection.None:
                    return true;
                case SyncronizeDirection.OnceToUI:
                case SyncronizeDirection.ToUI:
                    return flags.HasFlag(SyncronizeDirectionFlags.ToUI);
                case SyncronizeDirection.ToData:
                    return flags.HasFlag(SyncronizeDirectionFlags.ToData);
                case SyncronizeDirection.TwoWay:
                    return flags.HasFlag(SyncronizeDirectionFlags.Both);
            }
            return false;
        }
    }
}
