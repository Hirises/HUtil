using UnityEngine;
using System;

namespace HUtil.UI.Binder
{
    /// <summary>
    /// 바인딩 가능한 타입을 명시하는 인터페이스
    /// </summary>
    /// <typeparam name="T">바인딩 가능한 타입</typeparam>
    public interface IBinder<T>
    {
        /// <summary>
        /// 데이터를 바인딩합니다.
        /// </summary>
        /// <param name="data">바인딩할 데이터</param>
        void Bind(T data);
    }
}
