using System;
using System.Collections.Generic;

using UnityEngine;

namespace HUtil.UI.Converter
{
    public class BoolToggleConverter : MonoConverter
    {
        [SerializeField]
        private PropertyBindingPort<bool> _propertyBindingPort = new PropertyBindingPort<bool>(BindingType.Bool, BindingDirectionFlags.ToUI);
        [SerializeField]
        private string _outputPath = "Toggle";

        private IViewModelProperty _property;

        protected override void OnConvertBindingInfos(Dictionary<string, BindingInfo> bindingInfos)
        {
            bindingInfos[_outputPath] = new BindingInfo(_outputPath, BindingType.Bool, BindingDirectionFlags.ToUI);
        }

        protected override void OnConvertProperties(Dictionary<string, IViewModelProperty> bindMap)
        {
            ConvertProperty(bindMap, _propertyBindingPort.Path, _outputPath, (bool value) => !value, ref _property);
        }

        protected override void OnRestoreProperties(Dictionary<string, IViewModelProperty> bindMap)
        {
            RestoreProperty(bindMap, _outputPath, ref _property);
        }
    }
}