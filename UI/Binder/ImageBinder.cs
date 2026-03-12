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
        [SerializeField] private PropertyBindingPort _image_prop = new PropertyBindingPort(BindingType.OfType(BindingBaseType.Sprite), BindingDirectionFlags.ToUI);
        [SerializeField] private PropertyBindingPort _color_prop = new PropertyBindingPort(BindingType.OfType(BindingBaseType.Color), BindingDirectionFlags.ToUI);

        protected override void Reset()
        {
            base.Reset();
            _target = GetComponent<Image>();
        }

        protected void OnValidate()
        {
            _image_prop.Validate(this);
            _color_prop.Validate(this);
        }

        protected override void BindInternal(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable)
        {
            _image_prop.Bind<Sprite>(bindMap, disposable, SetSprite);
            _color_prop.Bind<Color>(bindMap, disposable, SetColor);
        }

        private void SetSprite(Sprite value)
        {
            _target.sprite = value;
        }

        private void SetColor(Color value)
        {
            _target.color = value;
        }
    }
}