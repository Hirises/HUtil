using System;
using System.Collections.Generic;

using HUtil.Runtime.Command;
using HUtil.Runtime.Observable;
using HUtil.UI.Binder;
using HUtil.UI.Converter;

using Sirenix.OdinInspector;

using UnityEngine;

namespace HUtil.UI.Processor
{
    public class PropertySetCommandRunner : MonoConverter
    {
        [SerializeField, OnValueChanged(nameof(AssignInputPort)), InlineProperty] private BindingType _inputType;
        [SerializeReference, HideReferenceObjectPicker, EnableIf(nameof(_inputPortAssigned))] private IAutoPropertyBindingPort _inputPort;
        [SerializeReference, HideReferenceObjectPicker, EnableIf(nameof(_inputPortAssigned))] private IConstantOrPropertyPort _valuePort;
        private bool _inputPortAssigned => _inputPort != null;
        [SerializeField] private string _commandPath;

        private IViewModelProperty _commandProperty;

        private void AssignInputPort(BindingType inputType){
            switch(inputType.BaseType){
                case BindingBaseType.Int:
                    _inputPort = new AutoPropertyBindingPort<int>(new PropertyBindingPort<int>(inputType, BindingDirectionFlags.ToData));
                    _valuePort = new ConstantOrPropertyPort<int>(new PropertyBindingPort<int>(inputType, BindingDirectionFlags.ToUI));
                    break;
                case BindingBaseType.Float:
                    _inputPort = new AutoPropertyBindingPort<float>(new PropertyBindingPort<float>(inputType, BindingDirectionFlags.ToData));
                    _valuePort = new ConstantOrPropertyPort<float>(new PropertyBindingPort<float>(inputType, BindingDirectionFlags.ToUI));
                    break;
                case BindingBaseType.Bool:
                    _inputPort = new AutoPropertyBindingPort<bool>(new PropertyBindingPort<bool>(inputType, BindingDirectionFlags.ToData));
                    _valuePort = new ConstantOrPropertyPort<bool>(new PropertyBindingPort<bool>(inputType, BindingDirectionFlags.ToUI));
                    break;
                case BindingBaseType.String:
                    _inputPort = new AutoPropertyBindingPort<string>(new PropertyBindingPort<string>(inputType, BindingDirectionFlags.ToData));
                    _valuePort = new ConstantOrPropertyPort<string>(new PropertyBindingPort<string>(inputType, BindingDirectionFlags.ToUI));
                    break;
                case BindingBaseType.Enum:
                    _inputPort = new AutoPropertyBindingPort<Enum>(new PropertyBindingPort<Enum>(inputType, BindingDirectionFlags.ToData));
                    _valuePort = new ConstantOrPropertyPort<Enum>(new PropertyBindingPort<Enum>(inputType, BindingDirectionFlags.ToUI));
                    break;
                case BindingBaseType.Color:
                    _inputPort = new AutoPropertyBindingPort<Color>(new PropertyBindingPort<Color>(inputType, BindingDirectionFlags.ToData));
                    _valuePort = new ConstantOrPropertyPort<Color>(new PropertyBindingPort<Color>(inputType, BindingDirectionFlags.ToUI));
                    break;
                case BindingBaseType.Sprite:
                    _inputPort = new AutoPropertyBindingPort<Sprite>(new PropertyBindingPort<Sprite>(inputType, BindingDirectionFlags.ToData));
                    _valuePort = new ConstantOrPropertyPort<Sprite>(new PropertyBindingPort<Sprite>(inputType, BindingDirectionFlags.ToUI));
                    break;
                case BindingBaseType.ViewModel:
                    _inputPort = new AutoPropertyBindingPort<IViewModel>(new PropertyBindingPort<IViewModel>(inputType, BindingDirectionFlags.ToData));
                    _valuePort = new ConstantOrPropertyPort<IViewModel>(new PropertyBindingPort<IViewModel>(inputType, BindingDirectionFlags.ToUI));
                    break;
                default:
                    _inputPort = null;
                    break;
            }
        }

        protected override void BindInternal(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable)
        {
            _valuePort?.Bind(bindMap, disposable);
            if(_inputPort != null){
                _inputPort.Bind(bindMap, disposable);
            }
        }

        protected override void OnConvertBindingInfos(Dictionary<string, BindingInfo> bindingInfos)
        {
            bindingInfos[_commandPath] = new BindingInfo(_commandPath, BindingType.Command, BindingDirectionFlags.ToData);
        }

        protected override void OnConvertProperties(Dictionary<string, IViewModelProperty> bindMap)
        {
            if(_inputPort == null){
                return;
            }
            if(bindMap.TryGetValue(_commandPath, out IViewModelProperty prop)){
                _commandProperty = prop;
            }
            switch(_inputType.BaseType){
                case BindingBaseType.Int:
                    bindMap[_commandPath] = new ResolvedProperty<RelayCommand>(new RelayCommand(() => {
                        if(_inputPort is AutoPropertyBindingPort<int> port && _valuePort is ConstantOrPropertyPort<int> valuePort){
                            port.SetValue(valuePort.GetValue());
                        }
                    }));
                    break;
                case BindingBaseType.Float:
                    bindMap[_commandPath] = new ResolvedProperty<RelayCommand>(new RelayCommand(() => {
                        if(_inputPort is AutoPropertyBindingPort<float> port && _valuePort is ConstantOrPropertyPort<float> valuePort){
                            port.SetValue(valuePort.GetValue());
                        }
                    }));
                    break;
                case BindingBaseType.Bool:
                    bindMap[_commandPath] = new ResolvedProperty<RelayCommand>(new RelayCommand(() => {
                        if(_inputPort is AutoPropertyBindingPort<bool> port && _valuePort is ConstantOrPropertyPort<bool> valuePort){
                            port.SetValue(valuePort.GetValue());
                        }
                    }));
                    break;
                case BindingBaseType.String:
                    bindMap[_commandPath] = new ResolvedProperty<RelayCommand>(new RelayCommand(() => {
                        if(_inputPort is AutoPropertyBindingPort<string> port && _valuePort is ConstantOrPropertyPort<string> valuePort){
                            port.SetValue(valuePort.GetValue());
                        }
                    }));
                    break;
                case BindingBaseType.Enum:
                    bindMap[_commandPath] = new ResolvedProperty<RelayCommand>(new RelayCommand(() => {
                        if(_inputPort is AutoPropertyBindingPort<Enum> port && _valuePort is ConstantOrPropertyPort<Enum> valuePort){
                            port.SetValue(valuePort.GetValue());
                        }
                    }));
                    break;
                case BindingBaseType.Color:
                    bindMap[_commandPath] = new ResolvedProperty<RelayCommand>(new RelayCommand(() => {
                        if(_inputPort is AutoPropertyBindingPort<Color> port && _valuePort is ConstantOrPropertyPort<Color> valuePort){
                            port.SetValue(valuePort.GetValue());
                        }
                    }));
                    break;
                case BindingBaseType.Sprite:
                    bindMap[_commandPath] = new ResolvedProperty<RelayCommand>(new RelayCommand(() => {
                        if(_inputPort is AutoPropertyBindingPort<Sprite> port && _valuePort is ConstantOrPropertyPort<Sprite> valuePort){
                            port.SetValue(valuePort.GetValue());
                        }
                    }));
                    break;
                case BindingBaseType.ViewModel:
                    bindMap[_commandPath] = new ResolvedProperty<RelayCommand>(new RelayCommand(() => {
                        if(_inputPort is AutoPropertyBindingPort<IViewModel> port && _valuePort is ConstantOrPropertyPort<IViewModel> valuePort){
                            port.SetValue(valuePort.GetValue());
                        }
                    }));
                    break;
            }
        }

        protected override void OnRestoreProperties(Dictionary<string, IViewModelProperty> bindMap)
        {
            RestoreProperty(bindMap, _commandPath, ref _commandProperty);
        }
    }
}