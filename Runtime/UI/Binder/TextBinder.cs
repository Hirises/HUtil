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
        [SerializeField] private string _propertyName;

        protected void Reset()
        {
            _target = GetComponent<TMP_Text>();
        }

        protected override void BindInternal(object viewModel, CompositeDisposable disposable)
        {
            var prop = ReflectionHelper.GetObservableProperty<string>(viewModel, _propertyName);
            prop.Subscribe(OnValueChanged).AddTo(disposable);
        }

        private void OnValueChanged(string value)
        {
            _target.text = value;
        }
    }
}
