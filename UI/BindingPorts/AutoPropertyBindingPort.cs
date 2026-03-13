using System;
using System.Collections.Generic;

using HUtil.Runtime.Observable;

namespace HUtil.UI
{
    public struct AutoPropertyBindingPort<T>
    {
        private PropertyBindingPort _propertyBindingPort;
        private T _bindConstant;
        private Action<T> _onValueChanged;

        public AutoPropertyBindingPort(PropertyBindingPort propertyBindingPort, Action<T> onValueChanged){
            _propertyBindingPort = propertyBindingPort;
            _bindConstant = default(T);
            _onValueChanged = onValueChanged;
        }

        public void Bind(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable){
            _propertyBindingPort.Bind<T>(bindMap, disposable, SetValue);
        }

        private void SetValue(T value){
            _bindConstant = value;
            _onValueChanged?.Invoke(value);
        }

        public T GetValue(){
            return _bindConstant;
        }
    }
}