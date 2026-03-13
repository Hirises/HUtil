using System;
using System.Collections.Generic;

using HUtil.Runtime.Observable;

using Sirenix.OdinInspector;

using UnityEngine;

namespace HUtil.UI.Converter
{
    public class ToStringConverter : MonoConverter
    {
        private enum ConditionType
        {
            Int,
            Long,
            Float,
            Double,
            Bool,
            Enum,
        }

        [SerializeField, OnValueChanged(nameof(OnTypeChanged))] private ConditionType _conditionType;
        [SerializeField, ShowIf(nameof(_conditionType), ConditionType.Int)] private PropertyBindingPort _int = new PropertyBindingPort(BindingType.OfType(BindingBaseType.Int), BindingDirectionFlags.ToUI);
        [SerializeField, ShowIf(nameof(_conditionType), ConditionType.Long)] private PropertyBindingPort _long = new PropertyBindingPort(BindingType.OfType(BindingBaseType.Long), BindingDirectionFlags.ToUI);
        [SerializeField, ShowIf(nameof(_conditionType), ConditionType.Float)] private PropertyBindingPort _float = new PropertyBindingPort(BindingType.OfType(BindingBaseType.Float), BindingDirectionFlags.ToUI);
        [SerializeField, ShowIf(nameof(_conditionType), ConditionType.Double)] private PropertyBindingPort _double = new PropertyBindingPort(BindingType.OfType(BindingBaseType.Double), BindingDirectionFlags.ToUI);
        [SerializeField, ShowIf(nameof(_conditionType), ConditionType.Bool)] private PropertyBindingPort _bool = new PropertyBindingPort(BindingType.OfType(BindingBaseType.Bool), BindingDirectionFlags.ToUI);
        [SerializeField, ShowIf(nameof(_conditionType), ConditionType.Enum)] private PropertyBindingPort _enum = new PropertyBindingPort(BindingType.OfType(BindingBaseType.Enum), BindingDirectionFlags.ToUI);
        [SerializeField] private string _outputPath;

        private IViewModelProperty _previousProperty;

        private void OnTypeChanged(){
            _int.Reset();
            _long.Reset();
            _float.Reset();
            _double.Reset();
            _bool.Reset();
            _enum.Reset();
        }

        protected override void OnConvertProperties(Dictionary<string, IViewModelProperty> bindMap)
        {
            switch(_conditionType)
            {
                case ConditionType.Int:
                    ConvertProperty<int, string>(bindMap, _int.Path, _outputPath, value => value.ToString(), ref _previousProperty);
                    break;
                case ConditionType.Float:
                    ConvertProperty<float, string>(bindMap, _float.Path, _outputPath, value => value.ToString(), ref _previousProperty);
                    break;
                case ConditionType.Enum:
                    ConvertProperty<Enum, string>(bindMap, _enum.Path, _outputPath, value => value.ToString(), ref _previousProperty);
                    break;
                case ConditionType.Long:
                    ConvertProperty<long, string>(bindMap, _long.Path, _outputPath, value => value.ToString(), ref _previousProperty);
                    break;
                case ConditionType.Double:
                    ConvertProperty<double, string>(bindMap, _double.Path, _outputPath, value => value.ToString(), ref _previousProperty);
                    break;
                case ConditionType.Bool:
                    ConvertProperty<bool, string>(bindMap, _bool.Path, _outputPath, value => value.ToString(), ref _previousProperty);
                    break;
            }
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