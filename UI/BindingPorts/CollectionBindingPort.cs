using System;
using System.Collections.Generic;
using System.Linq;

using HUtil.Runtime.Extension;
using HUtil.Runtime.Observable;
using HUtil.UI.Binder;

using UnityEngine;
using UnityEngine.Events;

namespace HUtil.UI
{
    [Serializable]
    public class CollectionBindingPort
    {       
        [SerializeField]
        private string _path;
        [SerializeField]
        private BindingMode _direction;
        [SerializeField]
        private BindingDirectionFlags _allowDirection;

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
        public BindingDirectionFlags AllowDirection => _allowDirection;

        /// <summary>
        /// 이 필드의 유효성을 검증합니다
        /// </summary>
        /// <returns>유효성 검증 결과. true일 경우 유효함</returns>
        public bool Validate(MonoBinder binder){
            if(!_allowDirection.CanAccept(Direction)){
                BindingContext.LogWarning($"Requested syncronize direction \"{Direction}\" is not allowed! this property only accpects {_allowDirection} direction", binder.gameObject);
                return false;
            }
            if(Direction != BindingMode.None && !string.IsNullOrEmpty(Path) && !binder.GetAllBindingInfosEditor().Any(info => info.Key == Path)){
                BindingContext.LogWarning($"Cannot find property {Path} in binder", binder.gameObject);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 인스펙터용 바인딩 속성을 생성합니다
        /// </summary>
        /// <param name="allowDirection">허용가능한 바인딩 방향</param>
        public CollectionBindingPort(BindingDirectionFlags allowDirection){
            _allowDirection = allowDirection;
        }

        public void Reset(){
            _path = "";
            _direction = BindingMode.None;
        }

        /// <summary>
        /// 현재 설정에 맞춰서 바인딩을 진행합니다
        /// </summary>
        /// <param name="bindMap">바인딩 맵</param>
        /// <param name="disposable">구독 관리용 disposable</param>
        /// <param name="setter">UI 값 setter</param>
        public void Bind(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable, Action<ListChangeEvent<IViewModel>> setter)
        {
            Bind(bindMap, disposable, setter, null);
        }

        /// <summary>
        /// 현재 설정에 맞춰서 바인딩을 진행합니다
        /// </summary>
        /// <param name="bindMap">바인딩 맵</param>
        /// <param name="disposable">구독 관리용 disposable</param>
        /// <param name="onChange">UI 값 변경 이벤트</param>
        public void Bind(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable, UnityEvent<ListChangeEvent<IViewModel>> onChange)
        {
            Bind(bindMap, disposable, null, onChange);
        }

        /// <summary>
        /// 현재 설정에 맞춰서 바인딩을 진행합니다
        /// </summary>
        /// <param name="bindMap">바인딩 맵</param>
        /// <param name="disposable">구독 관리용 disposable</param>
        /// <param name="setter">UI 값 setter</param>
        /// <param name="onChange">UI 값 변경 이벤트</param>
        public void Bind(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable, Action<ListChangeEvent<IViewModel>> setter, UnityEvent<ListChangeEvent<IViewModel>> onChange)
        {
            if(Direction == BindingMode.None){
                return;
            }
            if(!_allowDirection.CanAccept(Direction)){
                Debug.LogWarning($"[UIBinder] Requested syncronize direction \"{Direction}\" is not allowed! this property only accpects {_allowDirection} direction");
                return;
            }
            if(!bindMap.TryGetValue(Path, out var property)){
                Debug.LogWarning($"[UIBinder] Cannot find property {Path} in viewmodel");
                return;
            }

            switch (Direction)
            {
                case BindingMode.OnceToUI:
                {
                    if(setter == null) throw new ArgumentNullException(nameof(setter));

                    property.SubscribeList(setter).Dispose();
                    break;
                }
                case BindingMode.ToUI:
                {
                    if(setter == null) throw new ArgumentNullException(nameof(setter));

                    property.SubscribeList(setter).AddTo(disposable);
                    break;
                }
                case BindingMode.ToData:
                {
                    if(onChange == null) throw new ArgumentNullException(nameof(setter));

                    void listener(ListChangeEvent<IViewModel> @event) {
                        property.ApplyListChange(@event);
                    };
                    onChange.AddListener(listener);
                    new ScriptableDisposable(() => onChange.RemoveListener(listener)).AddTo(disposable);
                    break;
                }
                case BindingMode.TwoWay:
                {
                    if(setter == null) throw new ArgumentNullException(nameof(setter));
                    if(onChange == null) throw new ArgumentNullException(nameof(setter));

                    void listener(ListChangeEvent<IViewModel> @event) {
                        property.ApplyListChange(@event);
                    };
                    onChange.AddListener(listener);
                    new ScriptableDisposable(() => onChange.RemoveListener(listener)).AddTo(disposable);
                    property.SubscribeList(setter).AddTo(disposable);
                    break;
                }
            }
        }
    }
}