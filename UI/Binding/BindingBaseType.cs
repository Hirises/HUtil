using System;

namespace HUtil.UI
{
    /// <summary>
    /// 바인딩 대상 타입
    /// </summary>
    public enum BindingBaseType
    {
        None = 0,

        //기초 타입
        Int = 10,
        Float = 12,
        String = 14,
        Bool = 15,
        Enum = 16,

        Color = 34,
        Sprite = 37,

        //특수 타입
        Command = 70,
        ViewModel = 72,
    }
}