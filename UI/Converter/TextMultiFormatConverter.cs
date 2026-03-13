using System;
using System.Collections.Generic;
using System.Linq;

using HUtil.Runtime.Extension;
using HUtil.Runtime.Observable;

using Sirenix.OdinInspector;

using UnityEngine;

namespace HUtil.UI.Converter
{
    public class TextMultiFormatConverter : MonoConverter
    {
        [SerializeField] 
        private ConstantOrPropertyPort<string> _formatText = new ConstantOrPropertyPort<string>(new PropertyBindingPort(BindingType.OfType(BindingBaseType.String), BindingDirectionFlags.ToUI));
        [SerializeField] 
        [ListDrawerSettings(CustomAddFunction = nameof(CreateFormatArg))]
        private List<ConstantOrPropertyPort<string>> _formatArgs = new List<ConstantOrPropertyPort<string>>();

        [SerializeField] private string _outputPath;

        private IViewModelProperty _previousProperty;

        private ConstantOrPropertyPort<string> CreateFormatArg()
        {
            return new ConstantOrPropertyPort<string>(new PropertyBindingPort(BindingType.OfType(BindingBaseType.String), BindingDirectionFlags.ToUI));
        }

        private void OnChange(string str){

        }

        protected override void BindInternal(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable)
        {
            _formatText.Bind(bindMap, disposable);
            foreach(var arg in _formatArgs){
                arg.Bind(bindMap, disposable);
            }
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
            string formatText = _formatText.GetValue();
            _previousProperty = null;
            if(bindMap.TryGetValue(_outputPath, out var toProperty)){
                _previousProperty = toProperty;
            }
            //bindMap[_outputPath] = new FormattedStringProperty(fromProperty, _format, formatArgs);
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


            public FormattedStringProperty()
            {

            }

            public IDisposable SubscribeProperty<T>(Action<T> action)
            {
                action(GetPropertyValue<T>());
                return EmptyDisposable.Instance;
            }

            public T GetPropertyValue<T>()
            {
                return default(T);
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