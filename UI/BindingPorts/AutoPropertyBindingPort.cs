using System;
using System.Collections.Generic;

using HUtil.Runtime.Observable;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;

namespace HUtil.UI
{
    public interface IAutoPropertyBindingPort : IBindingPort{
        public void Bind(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable);
    }

    [Serializable, InlineProperty]
    public class AutoPropertyBindingPort<T> : IBindingPort, IAutoPropertyBindingPort
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

        public void SetValue(T value){
            _property.SetPropertyValue(value);
        }

        public void Bind(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable){
            if(Direction == BindingMode.None){
                return;
            }
            if(!_propertyBindingPort.AllowDirection.CanAccept(Direction)){
                Debug.LogWarning($"[UIBinder] Requested syncronize direction \"{Direction}\" is not allowed! this property only accpects {_propertyBindingPort.AllowDirection} direction");
                return;
            }
            if(!bindMap.TryGetValue(Path, out var rawProperty)){
                Debug.LogWarning($"[UIBinder] Cannot find property {Path} in viewmodel");
                return;
            }

            _property = rawProperty as IViewModelProperty<T>;
            disposable.Add(new ScriptableDisposable(() => _property = null));
        }
    }
}