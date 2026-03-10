using System;

namespace HUtil.Runtime.Observable
{
    /// <summary>
    /// 관찰 가능한 트리거
    /// </summary>
    public class ObservableTrigger : IReadOnlyObservableTrigger
    {
        private event Action _onTriggered;

        /// <summary>
        /// 이 객체를 읽기 전용으로 반환합니다
        /// </summary>
        /// <returns>읽기 전용 관찰 가능 트리거</returns>
        public IReadOnlyObservableTrigger AsReadOnly() => this;

        public ObservableTrigger(){

        }

        /// <summary>
        /// 트리거를 발생시킵니다
        /// </summary>
        public void Trigger()
        {
            _onTriggered?.Invoke();
        }

        /// <summary>
        /// 트리거가 발생했을 때 호출될 델리게이트를 구독합니다
        /// </summary>
        /// <param name="onTriggered">트리거가 발생했을 때 호출될 델리게이트</param>
        /// <returns>구독을 취소할 수 있는 IDisposable</returns>
        public IDisposable Subscribe(Action onTriggered)
        {
            if (onTriggered == null)
            {
                throw new ArgumentNullException(nameof(onTriggered), "구독자는 null일 수 없습니다!");
            }

            _onTriggered += onTriggered;
            return new ScriptableDisposable(() => _onTriggered -= onTriggered);
        }
    }
}