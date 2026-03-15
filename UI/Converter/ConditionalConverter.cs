using System;
using System.Collections.Generic;

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
            switch(outputType.BaseType){
                case BindingBaseType.String:
                    _outputPort = new PropertyBindingPort<string>(outputType, BindingDirectionFlags.ToUI);
                    break;
                default:
                    _outputPort = null;
                    break;
            }
        }

        protected override void OnConvertProperties(Dictionary<string, IViewModelProperty> bindMap)
        {
            throw new NotImplementedException();
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
    }
}