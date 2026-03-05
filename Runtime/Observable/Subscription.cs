using System;

namespace HUtil.Runtime.Observable
{
    /// <summary>
    /// 구독 취소를 제공하기 위한 IDisposable
    /// </summary>
    public class Subscription : IDisposable
    {
        private Action _unsubscribeAction;

        /// <summary>
        /// 구독 취소를 제공하기 위한 IDisposable를 생성합니다
        /// </summary>
        /// <param name="unsubscribeAction">구독 취소를 위한 액션</param>
        public Subscription(Action unsubscribeAction)
        {
            _unsubscribeAction = unsubscribeAction;
        }

        /// <summary>
        /// 구독 취소를 수행합니다
        /// </summary>
        public void Dispose()
        {
            _unsubscribeAction?.Invoke();
            _unsubscribeAction = null;
        }
    }
}