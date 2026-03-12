using System;

using HUtil.Runtime.Command;
using HUtil.Runtime.Observable;

namespace HUtil.UI
{
    public interface IViewModelProperty
    {
        /// <summary>
        /// 프로퍼티 값 변경 이벤트 구독
        /// </summary>
        /// <typeparam name="T">프로퍼티 타입</typeparam>
        /// <param name="action">변경 이벤트 핸들러</param>
        /// <returns>구독 해지 토큰</returns>
        public IDisposable SubscribeProperty<T>(Action<T> action);

        /// <summary>
        /// 프로퍼티 값 가져오기
        /// </summary>
        /// <typeparam name="T">프로퍼티 타입</typeparam>
        /// <returns>프로퍼티 값</returns>
        public T GetPropertyValue<T>();

        /// <summary>
        /// 프로퍼티 값 설정
        /// </summary>
        /// <typeparam name="T">프로퍼티 타입</typeparam>
        /// <param name="value"></param>
        public void SetPropertyValue<T>(T value);

        /// <summary>
        /// 명령 실행
        /// </summary>
        /// <param name="value">명령 파라미터</param>
        public void ExecuteCommand(object value);

        /// <summary>
        /// 리스트 변경 이벤트 구독
        /// </summary>
        /// <typeparam name="T">리스트 타입</typeparam>
        /// <param name="action">변경 이벤트 핸들러</param>
        /// <returns>구독 해지 토큰</returns>
        public IDisposable SubscribeList<T>(Action<ListChangeEvent<T>> action);

        /// <summary>
        /// 리스트 변경 이벤트 적용
        /// </summary>
        /// <typeparam name="T">리스트 타입</typeparam>
        /// <param name="event">적용할 리스트 변경 이벤트</param>
        public void ApplyListChange<T>(ListChangeEvent<T> @event);
    }
}