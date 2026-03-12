using System;

namespace HUtil.UI
{
    /// <summary>
    /// 바인딩 대상 타입
    /// </summary>
    public enum BindingType
    {
        None = 0,

        //기초 타입
        Int = 10,
        Long = 11,
        Float = 12,
        Double = 13,
        String = 14,
        Bool = 15,
        Enum = 16,

        //복합 타입
        Vector2 = 30,
        Vector3 = 31,
        Vector4 = 32,
        Quaternion = 33,
        Color = 34,
        Color32 = 35,
        DateTime = 36,

        //유니티 타입
        GameObject = 50,
        Transform = 51,

        //특수수 타입
        Command = 70,
        Trigger = 71,
        List = 72,
        ViewModel = 73,
    }
}