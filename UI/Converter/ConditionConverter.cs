using System;
using System.Collections.Generic;

using HUtil.Runtime.Observable;

using UnityEngine;

namespace HUtil.UI.Converter
{
    public class ConditionConverter : MonoConverter
    {
        [SerializeField] private PropertyBindingPort _condition = new PropertyBindingPort(BindingType.OfType(BindingBaseType.Int), BindingDirectionFlags.ToUI);
        [SerializeField] private string _outputPath;

        private IViewModelProperty _previousProperty;

        protected override void OnConvertProperties(Dictionary<string, IViewModelProperty> bindMap)
        {
            ConvertProperty<int, string>(bindMap, _condition.Path, _outputPath, value => value.ToString(), ref _previousProperty);
        }

        protected override void OnRestoreProperties(Dictionary<string, IViewModelProperty> bindMap)
        {
            RestoreProperty(bindMap, _outputPath, ref _previousProperty);
        }

        protected override void OnConvertBindingInfos(Dictionary<string, BindingInfo> bindingInfos)
        {
            bindingInfos[_outputPath] = new BindingInfo(_outputPath, BindingType.OfType(BindingBaseType.String), BindingDirectionFlags.ToUI);
        }
    }
}