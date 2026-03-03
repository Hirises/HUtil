using HUtil.Runtime.Observable;
using HUtil.Runtime.UI.Binder;

using UnityEngine;
using UnityEngine.UI;

namespace HUtil.Runtime.UI {
    public class InputFieldBinder : MonoBinder {
        [SerializeField] private InputField _target;
        [SerializeField] private PropertyBindingInfo _text_prop;

        protected void Reset()
        {
            _target = GetComponent<InputField>();
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
