using UnityEngine;
using System;
using TMPro;
using HUtil.Runtime.Extension;
using HUtil.Runtime.Observable;

namespace HUtil.Runtime.UI.Binder
{
    public class TextBinder : MonoBinder
    {
        [SerializeField] private TMP_Text _target;
        [SerializeField] private string _propertyName;

        private void Reset()
        {
            _target = GetComponent<TMP_Text>();
        }

        public override void Bind(object viewModel)
        {
            var prop = ReflectionHelper.GetObservableProperty<string>(viewModel, _propertyName);
            prop.Subscribe(OnValueChanged).AddTo(_disposable);
        }

        private void OnValueChanged(string value)
        {
            _target.text = value;
        }
    }
}
