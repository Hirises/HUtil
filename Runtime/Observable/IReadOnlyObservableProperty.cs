using System;

namespace HUtil.Runtime.Observable
{
    /// <summary>
    /// 읽기 전용 관찰 가능 속성 인터페이스
    /// </summary>
    /// <typeparam name="T">속성 타입</typeparam>
    public interface IReadOnlyObservableProperty<T>
    {
        /// <summary>
        /// 속성 값
        /// </summary>
        T Value { get; }

        /// <summary>
        /// 속성 값 변경 시 호출될 콜백 메서드 등록
        /// </summary>
        /// <param name="onValueChanged">콜백 메서드</param>
        /// <param name="notifyImmediately">즉시 1회 호출 여부</param>
        /// <returns>구독 해제를 위한 IDisposable</returns>
        IDisposable Subscribe(Action<T> onValueChanged, bool notifyImmediately = true);
    }
}