using System;
using System.Linq;

namespace HUtil.Runtime.UI
{
    /// <summary>
    /// 동기화 방향 확장 메서드
    /// </summary>
    public static class BindDirectionExtension
    {
        /// <summary>
        /// 입력된 동기화 방향이 허용되는지 여부를 반환합니다
        /// </summary>
        /// <param name="flags">허용되는 동기화 방향</param>
        /// <param name="direction">확인할 동기화 방향</param>
        /// <returns>혀용여부</returns>
        public static bool IsAllowed(this BindDirectionFlags flags, BindingMode direction){
            switch(direction){
                case BindingMode.None:
                    return true;
                case BindingMode.OnceToUI:
                case BindingMode.ToUI:
                    return flags.HasFlag(BindDirectionFlags.ToUI);
                case BindingMode.ToData:
                    return flags.HasFlag(BindDirectionFlags.ToData);
                case BindingMode.TwoWay:
                    return flags.HasFlag(BindDirectionFlags.Both);
            }
            return false;
        }
    }
}
