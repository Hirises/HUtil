using System;

namespace HUtil.Runtime.UI
{
    /// <summary>
    /// ViewModel의 바인딩 가능한 속성을 지정하는 어트리뷰트
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ViewModelValueAttribute : Attribute
    {
        /// <summary>
        /// 허용 가능한 바인딩 방향을 지정합니다
        /// </summary>
        public SyncronizeDirectionFlags _syncronizeDirection { get; private set; }

        public ViewModelValueAttribute(SyncronizeDirectionFlags syncronizeDirection = SyncronizeDirectionFlags.Both){
            this._syncronizeDirection = syncronizeDirection;
        }
    }
}
