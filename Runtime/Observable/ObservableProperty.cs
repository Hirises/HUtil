using System;
using System.Collections.Generic;

using UnityEngine;

namespace HUtil.Runtime.Observable
{
    /// <summary>
    /// 관측 가능한 프로퍼티 래퍼
    /// </summary>
    /// <typeparam name="T">대상 타입</typeparam>
    [Serializable]
    public class ObservableProperty<T> : IReadOnlyObservableProperty<T>
    {
        [SerializeField]
        private T _value;
        private event Action<T> _onValueChanged;
        public T Value
        {
            get => _value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value))
                {
                    return;
                }
                _value = value;
                _onValueChanged?.Invoke(_value); //오버헤드를 조금이라도 줄이기 위해서 Notify() 대신 직접 콜
            }
        }

        /// <summary>
        /// 이 객체를 읽기 전용으로 반환합니다
        /// </summary>
        /// <returns>읽기 전용 관찰 가능 속성</returns>
        public IReadOnlyObservableProperty<T> AsReadOnly() => this;

        public ObservableProperty(T initialValue = default)
        {
            _value = initialValue;
        }

        /// <summary>
        /// 해당 프로퍼티의 변화를 관찰합니다
        /// </summary>
        /// <param name="onValueChanged">콜백 메소드 (값 변경 이후에 호출됨)</param>
        /// <param name="notifyImmediately">즉시 1회 호출 여부</param>
        /// <returns>구독 해제를 위한 IDisposable</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IDisposable Subscribe(Action<T> onValueChanged, bool notifyImmediately = true)
        {
            if (onValueChanged == null)
            {
                throw new ArgumentNullException(nameof(onValueChanged), "구독자는 null일 수 없습니다!");
            }

            _onValueChanged += onValueChanged;
            if (notifyImmediately)
            {
                onValueChanged(_value);
            }
            return new Subscription(() => _onValueChanged -= onValueChanged);
        }

        /// <summary>
        /// 다른 관찰 가능 속성의 값을 이 속성에 반영합니다 (단방향 동기화)
        /// </summary>
        /// <param name="other">연결할 관찰 가능 속성</param>
        /// <returns>구독을 취소할 수 있는 <see cref="IDisposable"/></returns>
        public IDisposable Follow(IReadOnlyObservableProperty<T> other)
        {
            Value = other.Value;
            return new Subscription(() => other.Subscribe((v) => Value = v));
        }

        /// <summary>
        /// 다른 관찰 가능 속성과 이 속성을 동기화합니다 (양방향 동기화)
        /// </summary>
        /// <param name="other">동기화할 관찰 가능 속성</param>
        /// <returns>구독을 취소할 수 있는 <see cref="IDisposable"/></returns>
        public IDisposable Synchronize(ObservableProperty<T> other){
            var disposable = new CompositeDisposable();
            disposable.Add(this.Follow(other));
            disposable.Add(other.Follow(this));
            return disposable;
        }

        /// <summary>
        /// 해당 프로퍼티의 구독자들에게 강제로 메세지를 보냅니다
        /// </summary>
        public void Notify()
        {
            _onValueChanged?.Invoke(_value);
        }

        /// <summary>
        /// 변화를 알리지 않고 내부 값을 수정합니다
        /// </summary>
        /// <param name="value">수정할 값</param>
        public void SetValueWithoutNotify(T value)
        {
            _value = value;
        }

        public static implicit operator T(ObservableProperty<T> property) => property.Value;

        public override string ToString() => _value?.ToString();
    }   
}
