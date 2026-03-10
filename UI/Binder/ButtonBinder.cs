using UnityEngine;
using System;
using TMPro;
using HUtil.Runtime.Extension;
using HUtil.Runtime.Observable;
using UnityEngine.UI;
using System.Collections.Generic;

namespace HUtil.UI.Binder
{
    /// <summary>
    /// Button 바인딩 컴포넌트
    /// </summary>
    public class ButtonBinder : MonoBinder
    {
        [SerializeField] private Button _target;
        [SerializeField] private CommandBindingInfo _onClick_cmd = new CommandBindingInfo(BindDirectionFlags.ToData);

        protected override void Reset()
        {
            base.Reset();
            _target = GetComponent<Button>();
        }

        protected void OnValidate()
        {
            _onClick_cmd.Validate();
        }

        protected override void BindInternal(Dictionary<string, ResolvedProperty> bindMap, CompositeDisposable disposable)
        {
            _onClick_cmd.Bind(bindMap, disposable, _target.onClick);
        }
    }
}
