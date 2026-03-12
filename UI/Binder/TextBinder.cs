using UnityEngine;
using System;
using TMPro;
using HUtil.Runtime.Extension;
using HUtil.Runtime.Observable;
using System.Collections.Generic;

namespace HUtil.UI.Binder
{
    /// <summary>
    /// TMP_Text 바인딩 컴포넌트
    /// </summary>
    public class TextBinder : MonoBinder
    {
        [SerializeField] private TMP_Text _target;
        [SerializeField] private PropertyBindingPort _baseText_prop = new PropertyBindingPort(BindingBaseType.String, BindingDirectionFlags.ToUI);

        protected override void Reset()
        {
            base.Reset();
            _target = GetComponent<TMP_Text>();
        }

        protected virtual void OnValidate()
        {
            _baseText_prop.Validate();
        }

        protected override void BindInternal(Dictionary<string, ResolvedProperty> bindMap, CompositeDisposable disposable)
        {
            _baseText_prop.Bind<string>(bindMap, disposable, SetText);
        }

        protected virtual void SetText(string value)
        {
            _target.text = value;
        }
    }
}
