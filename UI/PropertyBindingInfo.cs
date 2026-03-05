using System;

using HUtil.Runtime.Extension;
using HUtil.Runtime.Observable;

using UnityEngine;
using UnityEngine.Events;

namespace HUtil.UI
{
    /// <summary>
    /// 프로퍼티 바인딩 인스펙터 속성
    /// </summary>
    [Serializable]
    public class PropertyBindingInfo
    {

        [SerializeField]
        private string _path;
        [SerializeField]
        private BindingMode _direction;

        private BindingType _receivingType;
        private BindDirectionFlags _allowDirection;

        /// <summary>
        /// 바인딩할 프로퍼티의 이름입니다
        /// </summary>
        public string Path => _path;
        /// <summary>
        /// 바인딩할 방향입니다
        /// </summary>
        public BindingMode Direction => _direction;
        /// <summary>
        /// 허용가능한 바인딩 방향입니다
        /// </summary>
        public BindDirectionFlags AllowDirection => _allowDirection;
        /// <summary>
        /// 이 프로퍼티가 받을 수 있는 타입입니다
        /// </summary>
        public BindingType ReceivingType => _receivingType;

        /// <summary>
        /// 이 필드의 유효성을 검증합니다
        /// </summary>
        /// <returns>유효성 검증 결과. true일 경우 유효함</returns>
        public bool Validate(){
            if(!_allowDirection.CanAccept(Direction)){
                Debug.LogWarning($"[UIBinder] Requested syncronize direction \"{Direction}\" is not allowed! this property only accpects {_allowDirection} direction");
                return false;
            }
            if(Direction != BindingMode.None && string.IsNullOrEmpty(Path)){
                Debug.LogWarning($"[UIBinder] Path is empty for this property setting!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 인스펙터용 바인딩 속성을 생성합니다
        /// </summary>
        /// <param name="allowDirection">허용가능한 바인딩 방향</param>
        public PropertyBindingInfo(BindingType receivingType, BindDirectionFlags allowDirection){
            _receivingType = receivingType;
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
            if(Direction == BindingMode.None){
                return;
            }

            if(!_allowDirection.CanAccept(Direction)){
                Debug.LogWarning($"[UIBinder] Requested syncronize direction \"{Direction}\" is not allowed! this property only accpects {_allowDirection} direction");
                return;
            }
            var observable = BinderReflectionHelper.GetObservableProperty<T>(viewModel, Path);
            if (observable == null)
            {
                Debug.LogWarning($"[UIBinder] Cannot find property {Path} in viewmodel");
                return;
            }
            switch (Direction)
            {
                case BindingMode.OnceToUI:
                {
                    if(setter == null) throw new ArgumentNullException(nameof(setter));

                    setter(observable.Value);
                    break;
                }
                case BindingMode.ToUI:
                {
                    if(setter == null) throw new ArgumentNullException(nameof(setter));

                    observable.Subscribe(setter).AddTo(disposable);
                    break;
                }
                case BindingMode.ToData:
                {
                    if(onChange == null) throw new ArgumentNullException(nameof(setter));

                    void listener(T value) {
                        observable.Value = value;
                    };
                    onChange.AddListener(listener);
                    new UnityEventSubscription(() => onChange.RemoveListener(listener)).AddTo(disposable);
                    break;
                }
                case BindingMode.TwoWay:
                {
                    if(setter == null) throw new ArgumentNullException(nameof(setter));
                    if(onChange == null) throw new ArgumentNullException(nameof(setter));

                    void listener(T value) {
                        observable.Value = value;
                    };
                    onChange.AddListener(listener);
                    new UnityEventSubscription(() => onChange.RemoveListener(listener)).AddTo(disposable);
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
