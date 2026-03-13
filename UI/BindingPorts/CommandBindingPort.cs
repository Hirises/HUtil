using System;
using System.Collections.Generic;
using System.Linq;

using HUtil.Runtime.Command;
using HUtil.Runtime.Extension;
using HUtil.Runtime.Observable;
using HUtil.UI.Binder;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;

namespace HUtil.UI
{
    /// <summary>
    /// 커맨드 바인딩 인스펙터 속성
    /// </summary>
    [Serializable, InlineProperty]
    public class CommandBindingPort
    {
        [SerializeField, HorizontalGroup, HideLabel, ValueDropdown(nameof(GetPossibleBindingModes)), OnValueChanged(nameof(OnDirectionChanged))]
        private BindingMode _direction;
        private List<BindingMode> GetPossibleBindingModes() => Enum.GetValues(typeof(BindingMode)).Cast<BindingMode>().Where(mode => _allowDirection.CanAccept(mode)).ToList();
        private void OnDirectionChanged(){
            if(_direction == BindingMode.None){
                _path = "";
            }
        }
        [SerializeField, DisableIf(nameof(Direction), BindingMode.None), HorizontalGroup, HideLabel]
        private string _path;
        [SerializeField, HideInInspector]
        private BindingDirectionFlags _allowDirection;

        /// <summary>
        /// 바인딩할 커맨드의 이름입니다
        /// </summary>
        public string Path => _path;
        /// <summary>
        /// 바인딩할 방향입니다
        /// </summary>
        public BindingMode Direction => _direction;
        /// <summary>
        /// 허용된 바인딩 방향입니다
        /// </summary>
        internal BindingDirectionFlags AllowDirection => _allowDirection;

        /// <summary>
        /// 이 필드의 유효성을 검증합니다
        /// </summary>
        /// <returns>유효성 검증 결과. true일 경우 유효함</returns>
        public bool Validate(MonoBinder binder){
            if(!_allowDirection.CanAccept(Direction)){
                BindingContext.LogWarning($"Requested syncronize direction \"{Direction}\" is not allowed! this property only accpects {_allowDirection} direction", binder.gameObject);
                return false;
            }
            if(Direction == BindingMode.OnceToUI){
                BindingContext.LogWarning($"OnceToUI direction is not allowed for command binding!", binder.gameObject);
                return false;
            }
            if(Direction != BindingMode.None && !string.IsNullOrEmpty(Path) && !binder.GetAllProvidingBindingInfosEditor().Any(info => info.Key == Path)){
                BindingContext.LogWarning($"Cannot find property {Path} in binder", binder.gameObject);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 인스펙터용 바인딩 속성을 생성합니다
        /// </summary>
        /// <param name="allowDirection">허용가능한 바인딩 방향</param>
        public CommandBindingPort(BindingDirectionFlags allowDirection){
            _allowDirection = allowDirection;
        }

        public void Reset(){
            _path = "";
            _direction = BindingMode.None;
        }

        /// <summary>
        /// 바인딩을 진행합니다
        /// </summary>
        /// <param name="bindMap">바인딩 맵</param>
        /// <param name="disposable">구독 관리용 disposable</param>
        /// <param name="onTrigger">UI 트리거 이벤트</param>
        /// <typeparam name="T">이벤트 파라미터 타입</typeparam>
        public void Bind<T>(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable, UnityEvent<T> onTrigger){
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
                case BindingMode.ToUI:
                case BindingMode.TwoWay:
                {
                    throw new NotSupportedException($"\"{Direction}\" direction is not allowed for command binding!");
                }
                case BindingMode.ToData:
                {
                    if(onTrigger == null) throw new ArgumentNullException(nameof(onTrigger));

                    void listener(T value) {
                        property.ExecuteCommand(value);
                    };
                    onTrigger.AddListener(listener);
                    new ScriptableDisposable(() => onTrigger.RemoveListener(listener)).AddTo(disposable);
                    break;
                }
            }
        }

        /// <summary>
        /// 현재 설정에 맞춰서 바인딩을 진행합니다
        /// </summary>
        /// <param name="bindMap">바인딩 맵</param>
        /// <param name="disposable">구독 관리용 disposable</param>
        /// <param name="onTrigger">UI 트리거 이벤트</param>
        public void Bind(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable, UnityEvent onTrigger)
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
                case BindingMode.ToUI:
                case BindingMode.TwoWay:
                {
                    throw new NotSupportedException($"\"{Direction}\" direction is not allowed for command binding!");
                }
                case BindingMode.ToData:
                {
                    if(onTrigger == null) throw new ArgumentNullException(nameof(onTrigger));

                    void listener() {
                        property.ExecuteCommand(null);
                    };
                    onTrigger.AddListener(listener);
                    new ScriptableDisposable(() => onTrigger.RemoveListener(listener)).AddTo(disposable);
                    break;
                }
            }
        }
    }
}
