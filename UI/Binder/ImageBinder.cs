using System;
using System.Collections.Generic;

using HUtil.Runtime.Observable;

using UnityEngine;
using UnityEngine.UI;

namespace HUtil.UI.Binder
{
    public class ImageBinder : MonoBinder
    {
        [SerializeField] private Image _target;
        [SerializeField] private PropertyBindingPort<Sprite> _image_prop = new PropertyBindingPort<Sprite>(BindingType.OfType(BindingBaseType.Sprite), BindingDirectionFlags.ToUI);
        [SerializeField] private PropertyBindingPort<Color> _color_prop = new PropertyBindingPort<Color>(BindingType.OfType(BindingBaseType.Color), BindingDirectionFlags.ToUI);
        [SerializeField] private PropertyBindingPort<float> _fill_prop = new PropertyBindingPort<float>(BindingType.OfType(BindingBaseType.Float), BindingDirectionFlags.ToUI);

        protected override void Reset()
        {
            base.Reset();
            _target = GetComponent<Image>();
        }

        protected void OnValidate()
        {
            _image_prop.Validate(this);
            _color_prop.Validate(this);
            _fill_prop.Validate(this);
        }

        protected override void BindInternal(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable)
        {
            _image_prop.Bind(bindMap, disposable, SetSprite);
            _color_prop.Bind(bindMap, disposable, SetColor);
            _fill_prop.Bind(bindMap, disposable, SetFill);
        }

        private void SetSprite(Sprite value)
        {
            _target.sprite = value;
        }

        private void SetColor(Color value)
        {
            _target.color = value;
        }

        private void SetFill(float value)
        {
            _target.fillAmount = value;
        }
    }
}