using System;

namespace HUtil.Runtime.UI
{
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

        ViewModel,
    }
}