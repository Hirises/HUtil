using System;

using HUtil.Runtime.Extension;
using HUtil.Runtime.Observable;

using UnityEngine;
using UnityEngine.Events;

namespace HUtil.Runtime.UI
{
    /// <summary>
    /// 프로퍼티 바인딩 인스펙터 속성
    /// </summary>
    [Serializable]
    public class PropertyBindingInfo
    {
        /// <summary>
        /// 바인딩할 프로퍼티의 이름입니다
        /// </summary>
        [field: SerializeField]
        public string Path { get; private set; }
        /// <summary>
        /// 바인딩할 방향입니다
        /// </summary>
        [field: SerializeField]
        public SyncronizeDirection Direction { get; private set; }

        private SyncronizeDirectionFlags _allowDirection;

        public PropertyBindingInfo(SyncronizeDirectionFlags allowDirection){
            _allowDirection = allowDirection;
        }

        /// <summary>
        /// 현재 설정에 맞춰서 바인딩을 진행합니다
        /// </summary>
        /// <typeparam name="T">대상 타입</typeparam>
        /// <param name="viewModel">뷰 모델 객체</param>
        /// <param name="disposable">구독 관리용 disposable</param>
        /// <param name="setter">UI 값 setter</param>
        public void Bind<T>(object viewModel, CompositeDisposable disposable, Action<T> setter)
        {
            Bind(viewModel, disposable, setter, null);
        }

        /// <summary>
        /// 현재 설정에 맞춰서 바인딩을 진행합니다
        /// </summary>
        /// <typeparam name="T">대상 타입</typeparam>
        /// <param name="viewModel">뷰 모델 객체</param>
        /// <param name="disposable">구독 관리용 disposable</param>
        /// <param name="onChange">UI 값 변경 이벤트</param>
        public void Bind<T>(object viewModel, CompositeDisposable disposable, UnityEvent<T> onChange)
        {
            Bind(viewModel, disposable, null, onChange);
        }

        /// <summary>
        /// 현재 설정에 맞춰서 바인딩을 진행합니다
        /// </summary>
        /// <typeparam name="T">대상 타입</typeparam>
        /// <param name="viewModel">뷰 모델 객체</param>
        /// <param name="disposable">구독 관리용 disposable</param>
        /// <param name="setter">UI 값 setter</param>
        /// <param name="onChange">UI 값 변경 이벤트</param>
        public void Bind<T>(object viewModel, CompositeDisposable disposable, Action<T> setter, UnityEvent<T> onChange)
        {
            if(!_allowDirection.IsAllowed(Direction)){
                Debug.LogWarning($"[UIBinder] Requested syncronize direction \"{Direction}\" is not allowed for this property setting! {_allowDirection}");
                return;
            }
            var observable = ReflectionHelper.GetObservableProperty<T>(viewModel, Path);
            if (observable == null)
            {
                Debug.LogWarning($"[UIBinder] Cannot find property {Path} in viewmodel");
                return;
            }
            switch (Direction)
            {
                case SyncronizeDirection.OneShotToTarget:
                {
                    if(setter == null) throw new ArgumentNullException(nameof(setter));

                    setter(observable.Value);
                    break;
                }
                case SyncronizeDirection.OneWayToTarget:
                {
                    if(setter == null) throw new ArgumentNullException(nameof(setter));

                    observable.Subscribe(setter).AddTo(disposable);
                    break;
                }
                case SyncronizeDirection.OneWayToSource:
                {
                    if(onChange == null) throw new ArgumentNullException(nameof(setter));

                    void reverse(T value) {
                        observable.Value = value;
                    };
                    onChange.AddListener(reverse);
                    new UnityEventSubscription(() => onChange.RemoveListener(reverse)).AddTo(disposable);
                    break;
                }
                case SyncronizeDirection.TwoWay:
                {
                    if(setter == null) throw new ArgumentNullException(nameof(setter));
                    if(onChange == null) throw new ArgumentNullException(nameof(setter));

                    void reverse(T value) {
                        observable.Value = value;
                    };
                    onChange.AddListener(reverse);
                    new UnityEventSubscription(() => onChange.RemoveListener(reverse)).AddTo(disposable);
                    observable.Subscribe(setter).AddTo(disposable);
                    break;
                }
            }
        }

        private class UnityEventSubscription : IDisposable
        {
            private Action _unsubscribeAction;

            public UnityEventSubscription(Action unsubscribeAction)
            {
                _unsubscribeAction = unsubscribeAction;
            }

            public void Dispose()
            {
                _unsubscribeAction?.Invoke();
                _unsubscribeAction = null;
            }
        }
    }
}
