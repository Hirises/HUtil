using System;

using UnityEngine;

namespace HUtil.UI
{
    /// <summary>
    /// ViewModel의 바인딩 가능한 속성을 지정하는 어트리뷰트
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class BindableAttribute : PropertyAttribute
    {
        private BindingDirectionFlags _allowedDirection;
        /// <summary>
        /// 허용 가능한 바인딩 방향을 지정합니다<br />
        /// Command는 이 설정의 영향을 받지 않습니다
        /// </summary>
        public BindingDirectionFlags AllowedDirection => _allowedDirection;

        /// <summary>
        /// 기본 생성자
        /// </summary>
        /// <param name="allowedDirection">허용 가능한 바인딩 방향<br />
        /// Command는 이 설정의 영향을 받지 않습니다</param>
        public BindableAttribute(BindingDirectionFlags allowedDirection = BindingDirectionFlags.Both){
            this._allowedDirection = allowedDirection;
        }
    }
}
