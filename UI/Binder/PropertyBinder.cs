using System;
using System.Collections.Generic;

using HUtil.Runtime.Observable;

using TMPro;

using UnityEngine;

namespace HUtil.UI.Binder
{
    public class PropertyBinder : MonoBinder
    {
        [SerializeField] private GameObject _target;
        [SerializeField] private AnimationClip _onAnimation;
        [SerializeField] private AnimationClip _offAnimation;
        [SerializeField] private PropertyBindingInfo _bool_prop = new PropertyBindingInfo(BindingType.Bool, BindingDirectionFlags.ToUI);

        protected override void Reset()
        {
            base.Reset();
            _target = gameObject;
        }

        protected void OnValidate()
        {
            _bool_prop.Validate();
        }

        protected override void BindInternal(Dictionary<string, ResolvedProperty> bindMap, CompositeDisposable disposable)
        {
            _bool_prop.Bind<bool>(bindMap, disposable, SetBool);
        }

        private void SetBool(bool value)
        {
            if (value)
            {
                _onAnimation?.SampleAnimation(_target, 0);
            }
            else
            {
                _offAnimation?.SampleAnimation(_target, 0);
            }
        }
    }
}