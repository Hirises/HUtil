using System;
using System.Collections.Generic;

using HUtil.Runtime;
using HUtil.Runtime.Extension;
using HUtil.Runtime.Observable;

using Sirenix.OdinInspector;

using UnityEngine;

namespace HUtil.UI.Converter
{
    public class ConditionalConverter : MonoConverter
    {
        [SerializeField, OnValueChanged(nameof(AssignInputPort)), InlineProperty] private BindingType _conditionType;
        [SerializeReference, HideReferenceObjectPicker, EnableIf(nameof(_inputPortAssigned))] private IBindingPort _inputPort;
        private bool _inputPortAssigned => _inputPort != null;
        
        [SerializeField, OnValueChanged(nameof(AssignOutputPort)), InlineProperty] private BindingType _outputType;
        private bool _outputTypeAssigned => _outputType.IsValid;
        [SerializeField, ListDrawerSettings(CustomAddFunction = nameof(AddConditionalBindingPort)), SerializeReference, EnableIf(nameof(_outputTypeAssigned)), HideReferenceObjectPicker] 
        private List<IConditionalBindingPort> _conditionalBindingPorts = new List<IConditionalBindingPort>();
        [SerializeField] private string _outputPath = "ConditionalValue";

        private IViewModelProperty previousProperty;

        private void AssignInputPort(BindingType conditionType){
            switch(conditionType.BaseType){
                case BindingBaseType.Int:
                    _inputPort = new PropertyBindingPort<int>(conditionType, BindingDirectionFlags.ToUI);
                    break;
                case BindingBaseType.Float:
                    _inputPort = new PropertyBindingPort<float>(conditionType, BindingDirectionFlags.ToUI);
                    break;
                case BindingBaseType.Bool:
                    _inputPort = new PropertyBindingPort<bool>(conditionType, BindingDirectionFlags.ToUI);
                    break;
                case BindingBaseType.String:
                    _inputPort = new PropertyBindingPort<string>(conditionType, BindingDirectionFlags.ToUI);
                    break;
                case BindingBaseType.Enum:
                    _inputPort = new PropertyBindingPort<Enum>(conditionType, BindingDirectionFlags.ToUI);
                    break;
                default:
                    _inputPort = null;
                    break;
            }
        }

        private void AssignOutputPort(BindingType outputType){
            _conditionalBindingPorts.Clear();
            if(outputType.BaseType == BindingBaseType.Command){
                _outputType = BindingType.Invalid;
            }
        }

        private IConditionalBindingPort AddConditionalBindingPort(){
            IConditionalBindingPort conditionalBindingPort = null;
            switch(_conditionType.BaseType){
                case BindingBaseType.Int:
                    conditionalBindingPort = new IntConditionalBindingPort();
                    break;
                case BindingBaseType.Float:
                    conditionalBindingPort = new FloatConditionalBindingPort();
                    break;
                case BindingBaseType.Bool:
                    conditionalBindingPort = new BoolConditionalBindingPort();
                    break;
                case BindingBaseType.String:
                    conditionalBindingPort = new StringConditionalBindingPort();
                    break;
                default:
                    return null;
            }
            conditionalBindingPort.TrueValue = _outputType.GetConstantOrPropertyPort();
            return conditionalBindingPort;
        }

        private T EvaluateConditional<T>(int conditionValue){
            foreach(var conditionalBindingPort in _conditionalBindingPorts){
                if(conditionalBindingPort is IntConditionalBindingPort intPort){
                    if(intPort.Operator.Compare(conditionValue, intPort.ConditionalValue.GetValue())){
                        var port = conditionalBindingPort.TrueValue as ConstantOrPropertyPort<T>;
                        return port != null ? port.GetValue() : default(T);
                    }
                }
            }
            return default(T);
        }

        private T EvaluateConditional<T>(float conditionValue){
            foreach(var conditionalBindingPort in _conditionalBindingPorts){
                if(conditionalBindingPort is FloatConditionalBindingPort floatPort){
                    if(floatPort.Operator.Compare(conditionValue, floatPort.ConditionalValue.GetValue())){
                        var port = conditionalBindingPort.TrueValue as ConstantOrPropertyPort<T>;
                        return port != null ? port.GetValue() : default(T);
                    }
                }
            }
            return default(T);
        }

        private T EvaluateConditional<T>(bool conditionValue){
            foreach(var conditionalBindingPort in _conditionalBindingPorts){
                if(conditionalBindingPort is BoolConditionalBindingPort boolPort){
                    if(boolPort.WhenEquals ^ (conditionValue != boolPort.ConditionalValue.GetValue())){
                        var port = conditionalBindingPort.TrueValue as ConstantOrPropertyPort<T>;
                        return port != null ? port.GetValue() : default(T);
                    }
                }
            }
            return default(T);
        }

        private T EvaluateConditional<T>(string conditionValue){
            foreach(var conditionalBindingPort in _conditionalBindingPorts){
                if(conditionalBindingPort is StringConditionalBindingPort stringPort){
                    if(stringPort.WhenEquals ^ (!conditionValue.Equals(stringPort.ConditionalValue.GetValue()))){
                        var port = conditionalBindingPort.TrueValue as ConstantOrPropertyPort<T>;
                        return port != null ? port.GetValue() : default(T);
                    }
                }
            }
            return default(T);
        }

        protected override void OnConvertProperties(Dictionary<string, IViewModelProperty> bindMap)
        {
            switch(_conditionType.BaseType){
                case BindingBaseType.Int:
                    switch(_outputType.BaseType){
                        case BindingBaseType.Int:
                            ConvertProperty<int, int>(bindMap, _inputPort.Path, _outputPath, (int value) => EvaluateConditional<int>(value), ref previousProperty);
                            break;
                        case BindingBaseType.Float:
                            ConvertProperty<int, float>(bindMap, _inputPort.Path, _outputPath, (int value) => EvaluateConditional<float>(value), ref previousProperty);
                            break;
                        case BindingBaseType.Bool:
                            ConvertProperty<int, bool>(bindMap, _inputPort.Path, _outputPath, (int value) => EvaluateConditional<bool>(value), ref previousProperty);
                            break;
                        case BindingBaseType.String:
                            ConvertProperty<int, string>(bindMap, _inputPort.Path, _outputPath, (int value) => EvaluateConditional<string>(value), ref previousProperty);
                            break;
                        default:
                            break;
                    }
                    break;
                case BindingBaseType.Float:
                    switch(_outputType.BaseType){
                        case BindingBaseType.Int:
                            ConvertProperty<float, int>(bindMap, _inputPort.Path, _outputPath, (float value) => EvaluateConditional<int>(value), ref previousProperty);
                            break;
                        case BindingBaseType.Float:
                            ConvertProperty<float, float>(bindMap, _inputPort.Path, _outputPath, (float value) => EvaluateConditional<float>(value), ref previousProperty);
                            break;
                        case BindingBaseType.Bool:
                            ConvertProperty<float, bool>(bindMap, _inputPort.Path, _outputPath, (float value) => EvaluateConditional<bool>(value), ref previousProperty);
                            break;
                        case BindingBaseType.String:
                            ConvertProperty<float, string>(bindMap, _inputPort.Path, _outputPath, (float value) => EvaluateConditional<string>(value), ref previousProperty);
                            break;
                        default:
                            break;
                    }
                    break;
                case BindingBaseType.Bool:
                    switch(_outputType.BaseType){
                        case BindingBaseType.Int:
                            ConvertProperty<bool, int>(bindMap, _inputPort.Path, _outputPath, (bool value) => EvaluateConditional<int>(value), ref previousProperty);
                            break;
                        case BindingBaseType.Float:
                            ConvertProperty<bool, float>(bindMap, _inputPort.Path, _outputPath, (bool value) => EvaluateConditional<float>(value), ref previousProperty);
                            break;
                        case BindingBaseType.Bool:
                            ConvertProperty<bool, bool>(bindMap, _inputPort.Path, _outputPath, (bool value) => EvaluateConditional<bool>(value), ref previousProperty);
                            break;
                        case BindingBaseType.String:
                            ConvertProperty<bool, string>(bindMap, _inputPort.Path, _outputPath, (bool value) => EvaluateConditional<string>(value), ref previousProperty);
                            break;
                        default:
                            break;
                    }
                    break;
                case BindingBaseType.String:
                    switch(_outputType.BaseType){
                        case BindingBaseType.Int:
                            ConvertProperty<string, int>(bindMap, _inputPort.Path, _outputPath, (string value) => EvaluateConditional<int>(value), ref previousProperty);
                            break;
                        case BindingBaseType.Float:
                            ConvertProperty<string, float>(bindMap, _inputPort.Path, _outputPath, (string value) => EvaluateConditional<float>(value), ref previousProperty);
                            break;
                        case BindingBaseType.Bool:
                            ConvertProperty<string, bool>(bindMap, _inputPort.Path, _outputPath, (string value) => EvaluateConditional<bool>(value), ref previousProperty);
                            break;
                        case BindingBaseType.String:
                            ConvertProperty<string, string>(bindMap, _inputPort.Path, _outputPath, (string value) => EvaluateConditional<string>(value), ref previousProperty);
                            break;
                        default:
                            break;
                    }
                    break;
                case BindingBaseType.Enum:
                    break;
            }
        }

        protected override void OnRestoreProperties(Dictionary<string, IViewModelProperty> bindMap)
        {
            RestoreProperty(bindMap, _outputPath, ref previousProperty);
        }

        protected override void OnConvertBindingInfos(Dictionary<string, BindingInfo> bindingInfos)
        {
            if(_inputPort == null){
                return;
            }
            bindingInfos[_outputPath] = new BindingInfo(
                _outputPath,
                _outputType,
                BindingDirectionFlags.ToUI
            );
        }

        protected override void BindInternal(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable)
        {
            base.BindInternal(bindMap, disposable);
            foreach(var conditionalBindingPort in _conditionalBindingPorts){
                if(conditionalBindingPort is IntConditionalBindingPort intPort){
                    intPort.ConditionalValue.Bind(bindMap, disposable);
                }
                else if(conditionalBindingPort is FloatConditionalBindingPort floatPort){
                    floatPort.ConditionalValue.Bind(bindMap, disposable);
                }
                else if(conditionalBindingPort is BoolConditionalBindingPort boolPort){
                    boolPort.ConditionalValue.Bind(bindMap, disposable);
                }
                else if(conditionalBindingPort is StringConditionalBindingPort stringPort){
                    stringPort.ConditionalValue.Bind(bindMap, disposable);
                }

                if(conditionalBindingPort.TrueValue is IConstantOrPropertyPort constantOrPropertyPort){
                    constantOrPropertyPort.Bind(bindMap, disposable);
                }
            }
            
            if(_inputPort is IConstantOrPropertyPort inputPort){
                inputPort.Bind(bindMap, disposable);
            }
        }

        private interface IConditionalBindingPort{
            public IBindingPort TrueValue { get; set; }
        }

        [Serializable, InlineProperty]
        private class IntConditionalBindingPort : IConditionalBindingPort
        {            
            [SerializeField] public ComparisonOperator Operator;
            [SerializeField, InlineProperty] public ConstantOrPropertyPort<int> ConditionalValue = new ConstantOrPropertyPort<int>(new PropertyBindingPort<int>(BindingType.Int, BindingDirectionFlags.ToUI));
            [SerializeField, SerializeReference, HideReferenceObjectPicker, InlineProperty] private IBindingPort _trueValue;
            public IBindingPort TrueValue { get => _trueValue; set => _trueValue = value; }
        }
        [Serializable, InlineProperty]
        private class FloatConditionalBindingPort : IConditionalBindingPort
        {
            [SerializeField] public ComparisonOperator Operator;
            [SerializeField, InlineProperty] public ConstantOrPropertyPort<float> ConditionalValue = new ConstantOrPropertyPort<float>(new PropertyBindingPort<float>(BindingType.Float, BindingDirectionFlags.ToUI));
            [SerializeField, SerializeReference, HideReferenceObjectPicker, InlineProperty] private IBindingPort _trueValue;
            public IBindingPort TrueValue { get => _trueValue; set => _trueValue = value; }
        }
        [Serializable, InlineProperty]
        private class BoolConditionalBindingPort : IConditionalBindingPort
        {
            [SerializeField] public bool WhenEquals = true;
            [SerializeField, InlineProperty] public ConstantOrPropertyPort<bool> ConditionalValue = new ConstantOrPropertyPort<bool>(new PropertyBindingPort<bool>(BindingType.Bool, BindingDirectionFlags.ToUI));
            [SerializeField, SerializeReference, HideReferenceObjectPicker, InlineProperty] private IBindingPort _trueValue;
            public IBindingPort TrueValue { get => _trueValue; set => _trueValue = value; }
        }
        [Serializable, InlineProperty]
        private class StringConditionalBindingPort : IConditionalBindingPort
        {
            [SerializeField] public bool WhenEquals = true;
            [SerializeField, InlineProperty] public ConstantOrPropertyPort<string> ConditionalValue = new ConstantOrPropertyPort<string>(new PropertyBindingPort<string>(BindingType.String, BindingDirectionFlags.ToUI));
            [SerializeField, SerializeReference, HideReferenceObjectPicker, InlineProperty] private IBindingPort _trueValue;
            public IBindingPort TrueValue { get => _trueValue; set => _trueValue = value; }
        }
    }
}