using HUtil.Runtime.Observable;
using HUtil.Runtime.UI.Binder;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace HUtil.Runtime.UI {
    public class InputFieldBinder : MonoBinder {
        [SerializeField] private TMP_InputField _target;
        [SerializeField] private PropertyBindingInfo _text_prop = new PropertyBindingInfo(BindingType.String, SyncronizeDirectionFlags.Both);

        protected override void Reset()
        {
            base.Reset();
            _target = GetComponent<TMP_InputField>();
        }

        protected void OnValidate()
        {
            _text_prop.Validate();
        }

        protected override void BindInternal(object viewModel, CompositeDisposable disposable)
        {
            _text_prop.Bind<string>(viewModel, disposable, SetText, _target.onValueChanged);
        }

        private void SetText(string value)
        {
            _target.text = value;
        }
    }
}
