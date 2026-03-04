using UnityEngine;
using System;
using TMPro;
using HUtil.Runtime.Extension;
using HUtil.Runtime.Observable;

namespace HUtil.Runtime.UI.Binder
{
    /// <summary>
    /// TMP_Text 바인딩 컴포넌트
    /// </summary>
    public class TextBinder : MonoBinder
    {
        [SerializeField] private TMP_Text _target;
        [SerializeField] private PropertyBindingInfo _text_prop = new PropertyBindingInfo(SyncronizeDirectionFlags.ToUI);

        protected override void Reset()
        {
            base.Reset();
            _target = GetComponent<TMP_Text>();
        }

        protected void OnValidate()
        {
            _text_prop.Validate();
        }

        protected override void BindInternal(object viewModel, CompositeDisposable disposable)
        {
            _text_prop.Bind<string>(viewModel, disposable, SetText);
        }

        private void SetText(string value)
        {
            _target.text = value;
        }
    }
}
