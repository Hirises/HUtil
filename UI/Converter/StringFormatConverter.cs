using System;
using System.Collections.Generic;
using System.Linq;

using HUtil.Runtime.Observable;

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

        protected override void BindInternal(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable)
        {
            base.BindInternal(bindMap, disposable);
            foreach(var parameter in _formatParameters){
                parameter.Bind(bindMap, disposable);
            }
            _format.Bind(bindMap, disposable);
        }

        protected override void OnConvertProperties(Dictionary<string, IViewModelProperty> bindMap)
        {
            var paths = _formatParameters.Where(parameter => !parameter.UseConstant).Select(parameter => parameter.Path).ToList();
            if(!_format.UseConstant){
                paths.Add(_format.Path);
            }
            ConvertMultiProperty<string, string>(bindMap, paths, _outputPath, () => {
                var parameters = _formatParameters.Select(parameter => parameter.GetValue()).ToArray();
                var formattedString = _format.GetValue();
                try{
                    formattedString = string.Format(formattedString, parameters);
                }
                catch(Exception e){
                    BindingContext.LogWarning($"Error formatting string: {e.Message}", gameObject);
                }
                return formattedString;
            }, ref previousProperty);
        }

        protected override void OnRestoreProperties(Dictionary<string, IViewModelProperty> bindMap)
        {
            RestoreProperty(bindMap, _outputPath, ref previousProperty);
        }
    }
}