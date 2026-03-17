using System;
using System.Collections.Generic;
using System.Linq;

using Sirenix.OdinInspector;

using UnityEngine;

namespace HUtil.UI.Converter
{
    public class StringFormatConverter : MonoConverter
    {
        [SerializeField] private ConstantOrPropertyPort<string> _format = new ConstantOrPropertyPort<string>(new PropertyBindingPort<string>(BindingType.String, BindingDirectionFlags.ToUI));
        [SerializeField, ListDrawerSettings(CustomAddFunction = nameof(AddFormatParameter))]
        private List<ConstantOrPropertyPort<string>> _formatParameters = new List<ConstantOrPropertyPort<string>>();
        [SerializeField] private string _outputPath = "StringFormatValue";

        private IViewModelProperty previousProperty;

        private static ConstantOrPropertyPort<string> AddFormatParameter()
        {
            return new ConstantOrPropertyPort<string>(new PropertyBindingPort<string>(BindingType.String, BindingDirectionFlags.ToUI));
        }

        protected override void OnConvertBindingInfos(Dictionary<string, BindingInfo> bindingInfos)
        {
            bindingInfos[_outputPath] = new BindingInfo(
                _outputPath,
                BindingType.String,
                BindingDirectionFlags.ToUI
            );
        }

        protected override void OnConvertProperties(Dictionary<string, IViewModelProperty> bindMap)
        {
            ConvertProperty<string, string>(bindMap, _format.Path, _outputPath, (string value) => string.Format(value, _formatParameters.Select(parameter => parameter.GetValue()).ToArray()), ref previousProperty);
        }

        protected override void OnRestoreProperties(Dictionary<string, IViewModelProperty> bindMap)
        {
            RestoreProperty(bindMap, _outputPath, ref previousProperty);
        }
    }
}