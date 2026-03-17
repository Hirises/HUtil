using System;
using System.Collections.Generic;

using HUtil.Runtime.Observable;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;

namespace HUtil.UI
{
    [Serializable, InlineProperty]
    public class AutoPropertyBindingPort<T> : IBindingPort
    {
        [SerializeField, InlineProperty, HideLabel] private PropertyBindingPort<T> _propertyBindingPort;
        private IViewModelProperty<T> _property;

        public string Path => _propertyBindingPort.Path;
        public BindingMode Direction => _propertyBindingPort.Direction;

        public AutoPropertyBindingPort(PropertyBindingPort<T> propertyBindingPort){
            _propertyBindingPort = propertyBindingPort;
            _property = null;
        }

        public T GetValue(){
            return _property.GetPropertyValue();
        }

        public void Bind(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable){
            this._property = _propertyBindingPort.Bind(bindMap, disposable, (T value) => {}, null);
            disposable.Add(new ScriptableDisposable(() => _property = null));
        }
    }
}