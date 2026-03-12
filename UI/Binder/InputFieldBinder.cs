using System.Collections.Generic;

using HUtil.Runtime.Observable;
using HUtil.UI.Binder;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace HUtil.UI.Binder {
    public class InputFieldBinder : MonoBinder {
        [SerializeField] private TMP_InputField _target;
        [SerializeField] private PropertyBindingPort _text_prop = new PropertyBindingPort(BindingType.OfType(BindingBaseType.String), BindingDirectionFlags.Both);

        protected override void Reset()
        {
            base.Reset();
            _target = GetComponent<TMP_InputField>();
        }

        protected void OnValidate()
        {
            _text_prop.Validate();
        }

        protected override void BindInternal(Dictionary<string, ResolvedProperty> bindMap, CompositeDisposable disposable)
        {
            _text_prop.Bind<string>(bindMap, disposable, SetText, _target.onValueChanged);
        }

        private void SetText(string value)
        {
            _target.text = value;
        }
    }
}
