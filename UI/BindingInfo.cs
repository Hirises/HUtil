using System;

namespace HUtil.UI
{
    /// <summary>
    /// ViewModel의 필드에 대한 바인딩 정보
    /// </summary>
    public struct BindingInfo
    {
        private string _propertyPath;
        private BindingType _type;
        private BindDirectionFlags _allowedDirection;

        /// <summary>
        /// 해당 필드의 이름
        /// </summary>
        public string PropertyPath => _propertyPath;
        /// <summary>
        /// 해당 필드의 타입
        /// </summary>
        public BindingType Type => _type;
        /// <summary>
        /// 해당 필드에 허용된 바인딩 방향
        /// </summary>
        public BindDirectionFlags AllowedDirection => _allowedDirection;

        /// <summary>
        /// 유효한 필드인지 여부
        /// </summary>
        public bool IsValid => Type != BindingType.None && AllowedDirection != BindDirectionFlags.None;

        public BindingInfo(string sourcePropertyPath, BindingType sourceType, BindDirectionFlags sourceDirection){
            _propertyPath = sourcePropertyPath;
            _type = sourceType;
            _allowedDirection = sourceDirection;
        }

        /// <summary>
        /// 해당 필드가 주어진 타입과 방향으로 바인딩 가능한지 확인합니다
        /// </summary>
        /// <param name="destinationType">받을 수 있는 타입</param>
        /// <param name="bindingMode">동기화 하려는 방향</param>
        /// <returns>바인딩 가능한지 여부</returns>
        public bool CanAccept(BindingType destinationType, BindingMode bindingMode){
            return Type.CanAccept(destinationType) && AllowedDirection.CanAccept(bindingMode);
        }
    }
}