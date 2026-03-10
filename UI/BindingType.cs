using System;

namespace HUtil.UI
{
    /// <summary>
    /// 바인딩 대상 타입
    /// </summary>
    public enum BindingType
    {
        None,

        Int,
        Long,
        Float,
        Double,
        String,
        Bool,

        Vector2,
        Vector3,
        Vector4,
        Quaternion,
        Color,
        Color32,
        DateTime,

        Enum,
        GameObject,
        Transform,
        Command,
        Trigger,
        List,

        ViewModel,
    }
}