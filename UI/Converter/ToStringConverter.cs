using System;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

namespace HUtil.UI.Converter
{
    public class ToStringConverter : MonoConverter
    {
        [SerializeField, OnValueChanged(nameof(AssignInputPort)), InlineProperty] private BindingType _inputType;
        [SerializeReference, HideReferenceObjectPicker, EnableIf(nameof(_inputPortAssigned))] private IBindingPort _inputPort;
        private bool _inputPortAssigned => _inputPort != null;
        [SerializeField] private string outputPath = "ToSTringValue";

        private IViewModelProperty previousProperty;

        private void AssignInputPort(BindingType inputType){
            switch(inputType.BaseType){
                case BindingBaseType.Int:
                    _inputPort = new PropertyBindingPort<int>(inputType, BindingDirectionFlags.ToUI);
                    break;
                case BindingBaseType.Float:
                    _inputPort = new PropertyBindingPort<float>(inputType, BindingDirectionFlags.ToUI);
                    break;
                case BindingBaseType.Bool:
                    _inputPort = new PropertyBindingPort<bool>(inputType, BindingDirectionFlags.ToUI);
                    break;
                default:
                    _inputPort = null;
                    break;
            }
        }

        protected override void OnConvertProperties(Dictionary<string, IViewModelProperty> bindMap)
        {
            if(_inputPort == null){
                return;
            }
                switch(_inputType.BaseType){
                case BindingBaseType.Int:
                    ConvertProperty<int, string>(bindMap, _inputPort.Path, outputPath, value => value.ToString(), ref previousProperty);
                    break;
                case BindingBaseType.Float:
                    ConvertProperty<float, string>(bindMap, _inputPort.Path, outputPath, value => value.ToString(), ref previousProperty);
                    break;
                case BindingBaseType.Bool:
                    ConvertProperty<bool, string>(bindMap, _inputPort.Path, outputPath, value => value.ToString(), ref previousProperty);
                    break;
                default:
                    _inputPort = null;
                    break;
            }
        }

        protected override void OnRestoreProperties(Dictionary<string, IViewModelProperty> bindMap)
        {
            RestoreProperty(bindMap, outputPath, ref previousProperty);
        }

        protected override void OnConvertBindingInfos(Dictionary<string, BindingInfo> bindingInfos){
            if(_inputPort == null){
                return;
            }
            bindingInfos[outputPath] = new BindingInfo(
                outputPath,
                BindingType.String,
                BindingDirectionFlags.ToUI
            );
        }
    }
}