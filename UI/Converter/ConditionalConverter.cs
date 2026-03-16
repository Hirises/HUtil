using System;
using System.Collections.Generic;

using HUtil.Runtime;

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

        protected override void OnConvertProperties(Dictionary<string, IViewModelProperty> bindMap)
        {
            // switch(_outputType.BaseType){
            //     case BindingBaseType.Int:
            //         _conditionalBindingPorts[0].TrueValue.Bind(bindMap, disposable, (int value) => _output.SetValue(value));
            //         break;
            //     case BindingBaseType.Float:
            //         _conditionalBindingPorts[0].TrueValue.Bind(bindMap, disposable, (float value) => _output.SetValue(value));
            //         break;
            // }
        }

        protected override void OnRestoreProperties(Dictionary<string, IViewModelProperty> bindMap)
        {
            throw new NotImplementedException();
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

        private interface IConditionalBindingPort{
            public IBindingPort TrueValue { get; set; }
        }

        [Serializable, InlineProperty]
        private class IntConditionalBindingPort : IConditionalBindingPort
        {
            [SerializeField, InlineProperty] public ConstantOrPropertyPort<int> Value = new ConstantOrPropertyPort<int>(new PropertyBindingPort<int>(BindingType.Int, BindingDirectionFlags.ToUI));
            [SerializeField] public ComparisonOperator Operator;
            [SerializeField, SerializeReference, HideReferenceObjectPicker, InlineProperty] private IBindingPort _trueValue;
            public IBindingPort TrueValue { get => _trueValue; set => _trueValue = value; }
        }
        [Serializable, InlineProperty]
        private class FloatConditionalBindingPort : IConditionalBindingPort
        {
            [SerializeField, InlineProperty] public ConstantOrPropertyPort<float> Value = new ConstantOrPropertyPort<float>(new PropertyBindingPort<float>(BindingType.Float, BindingDirectionFlags.ToUI));
            [SerializeField] public ComparisonOperator Operator;
            [SerializeField, SerializeReference, HideReferenceObjectPicker, InlineProperty] private IBindingPort _trueValue;
            public IBindingPort TrueValue { get => _trueValue; set => _trueValue = value; }
        }
        [Serializable, InlineProperty]
        private class BoolConditionalBindingPort : IConditionalBindingPort
        {
            [SerializeField, InlineProperty] public ConstantOrPropertyPort<bool> Value = new ConstantOrPropertyPort<bool>(new PropertyBindingPort<bool>(BindingType.Bool, BindingDirectionFlags.ToUI));
            [SerializeField] public bool WhenEquals;
            [SerializeField, SerializeReference, HideReferenceObjectPicker, InlineProperty] private IBindingPort _trueValue;
            public IBindingPort TrueValue { get => _trueValue; set => _trueValue = value; }
        }
        [Serializable, InlineProperty]
        private class StringConditionalBindingPort : IConditionalBindingPort
        {
            [SerializeField, InlineProperty] public ConstantOrPropertyPort<string> Value = new ConstantOrPropertyPort<string>(new PropertyBindingPort<string>(BindingType.String, BindingDirectionFlags.ToUI));
            [SerializeField] public bool WhenEquals;
            [SerializeField, SerializeReference, HideReferenceObjectPicker, InlineProperty] private IBindingPort _trueValue;
            public IBindingPort TrueValue { get => _trueValue; set => _trueValue = value; }
        }
    }
}