using System;
using System.Collections.Generic;

using HUtil.Runtime.L10N;
using HUtil.Runtime.Observable;

using TMPro;

using UnityEngine;

namespace HUtil.UI.Binder
{
    /// <summary>
    /// L10N이 적용된 TMP_Text 바인딩 컴포넌트
    /// </summary>
    public class L10NTextBinder : MonoBinder
    {
        [SerializeField] private TMP_Text _target;
        [SerializeField] private PropertyBindingPort _baseText_prop = new PropertyBindingPort(BindingType.String, BindingDirectionFlags.ToUI);

        protected override void Reset()
        {
            base.Reset();
            _target = GetComponent<TMP_Text>();
        }

        protected void OnValidate()
        {
            _baseText_prop.Validate();
        }

        protected override void BindInternal(Dictionary<string, ResolvedProperty> bindMap, CompositeDisposable disposable)
        {
            _baseText_prop.Bind<string>(bindMap, disposable, SetText);
        }

        private void SetText(string value)
        {
            _target.text = L10NConverter.ReplaceKey(value);
        }
    }
}