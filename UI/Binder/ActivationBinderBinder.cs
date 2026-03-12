using System;
using System.Collections.Generic;

using HUtil.Runtime.Observable;

using UnityEngine;

namespace HUtil.UI.Binder
{
    /// <summary>
    /// On/Off 그룹 바인더
    /// </summary>
    public class ActivationBinder : MonoBinder
    {
        [SerializeField] private GameObject[] _onObjects;
        [SerializeField] private GameObject[] _offObjects;
        [SerializeField] private PropertyBindingPort _isOn_prop = new PropertyBindingPort(BindingType.OfType(BindingBaseType.Bool), BindingDirectionFlags.ToUI);

        protected void OnValidate()
        {
            _isOn_prop.Validate(this);
        }

        protected override void BindInternal(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable)
        {
            _isOn_prop.Bind<bool>(bindMap, disposable, SetIsOn);
        }

        private void SetIsOn(bool isOn)
        {
            foreach (var obj in _onObjects)
            {
                obj.SetActive(isOn);
            }
            foreach (var obj in _offObjects)
            {
                obj.SetActive(!isOn);
            }
        }
    }
}