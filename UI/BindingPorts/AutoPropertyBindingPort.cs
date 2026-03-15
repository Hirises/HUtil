using System;
using System.Collections.Generic;

using HUtil.Runtime.Observable;

using UnityEngine.Events;

namespace HUtil.UI
{
    public class AutoPropertyBindingPort<T>
    {
        private PropertyBindingPort<T> _propertyBindingPort;
        private IViewModelProperty<T> _property;

        public AutoPropertyBindingPort(PropertyBindingPort<T> propertyBindingPort){
            _propertyBindingPort = propertyBindingPort;
            _property = null;
        }

        public T GetValue(){
            return _property.GetPropertyValue();
        }

        public void Bind(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable, Action<T> setter){
            Bind(bindMap, disposable, setter, null);
        }

        public void Bind(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable, UnityEvent<T> onChange){
            Bind(bindMap, disposable, null, onChange);
        }

        public void Bind(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable, Action<T> setter, UnityEvent<T> onChange){
            this._property = _propertyBindingPort.Bind(bindMap, disposable, setter, onChange);
            disposable.Add(new ScriptableDisposable(() => _property = null));
        }
    }
}