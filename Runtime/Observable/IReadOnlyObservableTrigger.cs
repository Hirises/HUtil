using System;

namespace HUtil.Runtime.Observable
{
    /// <summary>
    /// 읽기 전용 관찰 가능 트리거 인터페이스
    /// </summary>
    public interface IReadOnlyObservableTrigger
    {
        /// <summary>
        /// 트리거가 발생했을 때 호출될 델리게이트를 구독합니다
        /// </summary>
        /// <param name="onTriggered">트리거가 발생했을 때 호출될 델리게이트</param>
        /// <returns>구독을 취소할 수 있는 <see cref="IDisposable"/></returns>
        IDisposable Subscribe(Action onTriggered);
    }
}