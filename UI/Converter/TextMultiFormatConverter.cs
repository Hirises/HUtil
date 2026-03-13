using System;
using System.Collections.Generic;
using System.Linq;

using HUtil.Attribute;
using HUtil.Runtime.Extension;
using HUtil.Runtime.Observable;

using UnityEngine;

namespace HUtil.UI.Converter
{
    public class TextMultiFormatConverter : MonoConverter
    {
        [SerializeField] private bool _useDynamicFormat = false;
        [SerializeField, ShowIf(nameof(_useDynamicFormat))] private PropertyBindingPort _formatText = new PropertyBindingPort(BindingType.OfType(BindingBaseType.String), BindingDirectionFlags.ToUI);
        [SerializeField, HideIf(nameof(_useDynamicFormat))] private string _format;
        [SerializeField, ScriptableList(nameof(AddNewFormatArg))] private List<PropertyBindingPort> _formatArgs = new List<PropertyBindingPort>();
        [SerializeField] private string _outputPath;

        private IViewModelProperty _previousProperty;

        private void AddNewFormatArg(PropertyBindingPort instance)
        {
            instance.Initialize(BindingType.OfType(BindingBaseType.String), BindingDirectionFlags.ToUI);
        }

        protected override void OnConvertBindingInfos(Dictionary<string, BindingInfo> bindingInfos)
        {
            bindingInfos[_outputPath] = new BindingInfo(
                _outputPath,
                BindingType.OfType(BindingBaseType.String),
                BindingDirectionFlags.ToUI
            );
        }

        protected override void OnConvertProperties(Dictionary<string, IViewModelProperty> bindMap)
        {   
            IViewModelProperty fromProperty = null;
            if(_useDynamicFormat && !bindMap.TryGetValue(_formatText.Path, out fromProperty)){
                BindingContext.LogWarning($"{_formatText.Path} is not found", gameObject);
                return;
            }
            List<IViewModelProperty> formatArgs = new();
            foreach(var arg in _formatArgs){
                if(!bindMap.TryGetValue(arg.Path, out var argProperty)){
                    BindingContext.LogWarning($"{arg.Path} is not found", gameObject);
                    return;
                }
                formatArgs.Add(argProperty);
            }
            _previousProperty = null;
            if(bindMap.TryGetValue(_outputPath, out var toProperty)){
                _previousProperty = toProperty;
            }
            bindMap[_outputPath] = new FormattedStringProperty(fromProperty, _format, formatArgs);
        }

        protected override void OnRestoreProperties(Dictionary<string, IViewModelProperty> bindMap)
        {
            if(_previousProperty == null){
                bindMap.Remove(_outputPath);
            }else{
                bindMap[_outputPath] = _previousProperty;
            }
        }

        public class FormattedStringProperty : IViewModelProperty
        {
            private IViewModelProperty _baseString;
            private string _staticFormat;
            private List<IViewModelProperty> _formatArgs;
            private CompositeDisposable _disposables;
            private Action<string> _setter;

            public FormattedStringProperty(IViewModelProperty baseString, string staticFormat, List<IViewModelProperty> formatArgs)
            {
                _baseString = baseString;
                _staticFormat = staticFormat ?? throw new ArgumentNullException(nameof(staticFormat));
                _formatArgs = formatArgs ?? throw new ArgumentNullException(nameof(formatArgs));
                _disposables = new CompositeDisposable();
            }

            public IDisposable SubscribeProperty<T>(Action<T> action)
            {
                _setter = (string value) => action((T)(object)value);
                _baseString?.SubscribeProperty<string>(value => UpdateString()).AddTo(_disposables);
                foreach(var arg in _formatArgs){
                    arg.SubscribeProperty<string>(value => UpdateString()).AddTo(_disposables);
                }
                return _disposables;
            }

            public void UpdateString(){
                string baseString = _baseString?.GetPropertyValue<string>() ?? _staticFormat;
                string formattedString = string.Format(baseString, _formatArgs.Select(arg => arg.GetPropertyValue<string>()).ToArray());
                _setter?.Invoke(formattedString);
            }

            public T GetPropertyValue<T>()
            {
                return (T)(object)_baseString.GetPropertyValue<T>();
            }

            public void SetPropertyValue<T>(T value)
            {
                Debug.LogWarning($"[UIBinder] You cannot write to a modified property!");
            }

            public void ExecuteCommand(object value)
            {
                Debug.LogWarning($"[UIBinder] You cannot execute command to a modified property!");
                return;
            }

            public IDisposable SubscribeList<T>(Action<ListChangeEvent<T>> action)
            {
                Debug.LogWarning($"[UIBinder] You cannot subscribe list to a modified property!");
                return EmptyDisposable.Instance;
            }

            public void ApplyListChange<T>(ListChangeEvent<T> @event)
            {
                Debug.LogWarning($"[UIBinder] You cannot apply list change to a modified property!");
                return;
            }
        }
    }
}