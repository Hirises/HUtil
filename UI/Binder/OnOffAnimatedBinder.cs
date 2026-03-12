using System;
using System.Collections.Generic;

using HUtil.Runtime.Observable;

using TMPro;

using UnityEngine;

namespace HUtil.UI.Binder
{
    /// <summary>
    /// 동적 애니메이션 클립 생성을 이용한 멀티 바인더
    /// </summary>
    public class OnOffAnimatedBinder : MonoBinder
    {
        [SerializeField] private GameObject _target;
        [SerializeField] private AnimationClip _onAnimation;
        [SerializeField] private AnimationClip _offAnimation;
        [SerializeField] private PropertyBindingPort _isOn_prop = new PropertyBindingPort(BindingType.Bool, BindingDirectionFlags.ToUI);

        protected override void Reset()
        {
            base.Reset();
            _target = gameObject;
        }

        protected void OnValidate()
        {
            _isOn_prop.Validate();
        }

        protected override void BindInternal(Dictionary<string, ResolvedProperty> bindMap, CompositeDisposable disposable)
        {
            _isOn_prop.Bind<bool>(bindMap, disposable, SetBool);
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