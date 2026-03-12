using UnityEngine;
using System;
using HUtil.UI.Binder;
using System.Collections.Generic;
using HUtil.Runtime.Observable;
using HUtil.Runtime;
using HUtil.Runtime.Extension;

namespace HUtil.UI
{
    /// <summary>
    /// 입력값을 bool값으로 변환하는 컨버터
    /// </summary>
    public class ConditionalConverter : MonoBinder
    {
        protected override bool IsRootBinder => true;

        [SerializeField] private PropertyBindingPort _int_prop = new PropertyBindingPort(BindingType.OfType(BindingBaseType.Int), BindingDirectionFlags.ToUI);
        [SerializeField] private PropertyBindingPort _float_prop = new PropertyBindingPort(BindingType.OfType(BindingBaseType.Float), BindingDirectionFlags.ToUI);
        [SerializeField] private PropertyBindingPort _bool_prop = new PropertyBindingPort(BindingType.OfType(BindingBaseType.Bool), BindingDirectionFlags.ToUI);
        [SerializeField] private PropertyBindingPort _string_prop = new PropertyBindingPort(BindingType.OfType(BindingBaseType.String), BindingDirectionFlags.ToUI);
        [SerializeField] private string _output_path = "condition";

        [SerializeField] private ComparisonOperator _comparisonOperator = ComparisonOperator.Equal;   
        [SerializeField] private string _comparisonString = "value";
        [SerializeField] private float _comparisonFloat = 0f;
        [SerializeField] private int _comparisonInt = 0;
        private IViewModelProperty _previous_property = null;
        

        protected override void BindInternal(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable)
        {
            //pass
        }

        protected override void BeforePropagate(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable)
        {
            _previous_property = null;
            if(bindMap.TryGetValue(_int_prop.Path, out var property)){
                _previous_property = bindMap.GetValueOrDefault(_output_path, null);
                var observable = property.AsObservableProperty<int>() as ObservableProperty<int>;
                var prop = VirtualProperty<bool>.Create(observable, (v) => _comparisonOperator.Compare(v, _comparisonInt), disposable);
                bindMap[_output_path] = prop;
            }
        }

        protected override void AfterPropagate(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable)
        {
            if(_previous_property != null){
                bindMap[_output_path] = _previous_property;
            }else{
                bindMap.Remove(_output_path);
            }
        }

        internal override List<BindingInfo> GetAllBindingInfosEditor()
        {
            var upperBind = base.GetAllBindingInfosEditor();
            upperBind.Add(new BindingInfo(_output_path, BindingType.OfType(BindingBaseType.Bool), BindingDirectionFlags.ToUI));
            return upperBind;
        }
    }
}
