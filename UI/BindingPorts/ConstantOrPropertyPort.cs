using System;
using System.Collections.Generic;

using HUtil.Runtime.Observable;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;

namespace HUtil.UI
{
    [Serializable]
    public class ConstantOrPropertyPort<T> : IBindingPort
    {
        [SerializeField] private bool _useConstant;
        [SerializeField, HideIf(nameof(_useConstant))] private AutoPropertyBindingPort<T> _autoPropertyBindingPort;
        [SerializeField, ShowIf(nameof(_useConstant))] private T _constant;

        public string Path => _autoPropertyBindingPort.Path;
        public BindingMode Direction => _autoPropertyBindingPort.Direction;

        public ConstantOrPropertyPort(PropertyBindingPort<T> propertyBindingPort, Action<T> onValueChanged = null){
            _autoPropertyBindingPort = new AutoPropertyBindingPort<T>(propertyBindingPort);
            _constant = default(T);
            _useConstant = true;
        }

        public T GetValue(){
            return _useConstant ? _constant : _autoPropertyBindingPort.GetValue();
        }

        public void Bind(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable, Action<T> setter){
            _autoPropertyBindingPort.Bind(bindMap, disposable, setter);
        }

        public void Bind(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable, UnityEvent<T> onChange){
            _autoPropertyBindingPort.Bind(bindMap, disposable, onChange);
        }

        public void Bind(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable, Action<T> setter, UnityEvent<T> onChange){
            _autoPropertyBindingPort.Bind(bindMap, disposable, setter, onChange);
        }
    }
}