using System;
using System.Collections.Generic;

using HUtil.Runtime.Observable;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;

namespace HUtil.UI
{
    public interface IConstantOrPropertyPort{
        public bool UseConstant { get; }
        public void Bind(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable);
    }

    [Serializable]
    public class ConstantOrPropertyPort<T> : IBindingPort, IConstantOrPropertyPort
    {
        [SerializeField] private bool _useConstant;
        [SerializeField, HideIf(nameof(_useConstant))] private AutoPropertyBindingPort<T> _autoPropertyBindingPort;
        [SerializeField, ShowIf(nameof(_useConstant))] private T _constant;

        public string Path => _autoPropertyBindingPort.Path;
        public BindingMode Direction => _autoPropertyBindingPort.Direction;
        public bool UseConstant => _useConstant;

        public ConstantOrPropertyPort(PropertyBindingPort<T> propertyBindingPort, Action<T> onValueChanged = null){
            _autoPropertyBindingPort = new AutoPropertyBindingPort<T>(propertyBindingPort);
            _constant = default(T);
            _useConstant = true;
        }

        public T GetValue(){
            return _useConstant ? _constant : _autoPropertyBindingPort.GetValue();
        }

        public void Bind(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable){
            if(!_useConstant) _autoPropertyBindingPort.Bind(bindMap, disposable);
        }
    }
}