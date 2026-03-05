using System;
using System.Linq;

namespace HUtil.UI
{
    /// <summary>
    /// 동기화 방향 확장 메서드
    /// </summary>
    public static class BindDirectionExtension
    {
        /// <summary>
        /// 주어진 동기화 플래그(flags)에서 특정 동기화 방향(direction)이 허용되는지 확인합니다.
        /// </summary>
        /// <param name="flags">현재 설정된 동기화 방향 플래그</param>
        /// <param name="direction">체크할 동기화 방향</param>
        /// <returns>해당 방향이 허용되면 true, 아니면 false를 반환</returns>
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
