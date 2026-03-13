using System;
using System.Collections.Generic;

using HUtil.Runtime.Observable;

namespace HUtil.UI
{
    public struct ConstantOrPropertyPort<T>
    {
        private bool _useConstant;
        private AutoPropertyBindingPort<T> _autoPropertyBindingPort;
        private T _constant;

        public ConstantOrPropertyPort(PropertyBindingPort propertyBindingPort, Action<T> onValueChanged = null){
            _autoPropertyBindingPort = new AutoPropertyBindingPort<T>(propertyBindingPort, onValueChanged);
            _constant = default(T);
            _useConstant = true;
        }

        public void Bind(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable){
            _autoPropertyBindingPort.Bind(bindMap, disposable);
        }

        public T GetValue(){
            return _useConstant ? _constant : _autoPropertyBindingPort.GetValue();
        }
    }
}