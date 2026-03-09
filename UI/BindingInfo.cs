using System;

namespace HUtil.UI
{
    public struct BindingInfo
    {
        public string SourcePropertyPath;
        public BindingType SourceType;
        public BindDirectionFlags SourceDirection;

        public bool IsValid => SourceType != BindingType.None && SourceDirection != BindDirectionFlags.None;

        public BindingInfo(string sourcePropertyPath, BindingType sourceType, BindDirectionFlags sourceDirection){
            SourcePropertyPath = sourcePropertyPath;
            SourceType = sourceType;
            SourceDirection = sourceDirection;
        }

        public bool CanAccept(BindingType destinationType, BindingMode bindingMode){
            return SourceType.CanAccept(destinationType) && SourceDirection.CanAccept(bindingMode);
        }
    }
}