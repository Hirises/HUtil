using System;
using System.Collections.Generic;

using HUtil.Runtime.Command;
using HUtil.Runtime.Extension;
using HUtil.Runtime.Observable;

using UnityEngine;
using UnityEngine.Events;

namespace HUtil.UI
{
    /// <summary>
    /// 커맨드 바인딩 인스펙터 속성
    /// </summary>
    [Serializable]
    public class CommandBindingInfo
    {
        [SerializeField]
        private string _path;
        [SerializeField]
        private BindingMode _direction;
        [SerializeField]
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
        public bool Validate(){
            if(!_allowDirection.CanAccept(Direction)){
                Debug.LogWarning($"[UIBinder] Requested syncronize direction \"{Direction}\" is not allowed! this property only accpects {_allowDirection} direction");
                return false;
            }
            if(Direction == BindingMode.OnceToUI){
                Debug.LogWarning($"[UIBinder] OnceToUI direction is not allowed for command binding!");
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
        public CommandBindingInfo(BindingDirectionFlags allowDirection){
            _allowDirection = allowDirection;
        }

        /// <summary>
        /// 현재 설정에 맞춰서 바인딩을 진행합니다
        /// </summary>
        /// <param name="bindMap">바인딩 맵</param>
        /// <param name="disposable">구독 관리용 disposable</param>
        /// <param name="onTrigger">UI 트리거 이벤트</param>
        public void Bind(Dictionary<string, ResolvedProperty> bindMap, CompositeDisposable disposable, UnityEvent onTrigger)
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
            var command = property.AsCommand();
            if (command == null)
            {
                Debug.LogWarning($"[UIBinder] Property {Path} is not a command!");
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
                    if(command == null) throw new ArgumentNullException(nameof(command));
                    if(onTrigger == null) throw new ArgumentNullException(nameof(onTrigger));

                    void listener() {
                        command.Execute(null);
                    };
                    onTrigger.AddListener(listener);
                    new ScriptableDisposable(() => onTrigger.RemoveListener(listener)).AddTo(disposable);
                    break;
                }
            }
        }
    }
}
