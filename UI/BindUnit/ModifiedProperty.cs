using System;

using HUtil.Runtime.Observable;

using UnityEngine;

namespace HUtil.UI
{
    public struct ModifiedProperty<From, To> : IViewModelProperty<To>
    {
        private IViewModelProperty<From> _origin;
        private Func<From, To> _modifier;

        public ModifiedProperty(IViewModelProperty<From> origin, Func<From, To> modifier)
        {
            _origin = origin ?? throw new ArgumentNullException(nameof(origin));
            _modifier = modifier ?? throw new ArgumentNullException(nameof(modifier));
        }

        public IDisposable SubscribeProperty(Action<To> action)
        {
            var modifier = _modifier;
            return _origin.SubscribeProperty(value => action(modifier(value)));
        }

        public To GetPropertyValue()
        {
            return _modifier(_origin.GetPropertyValue());
        }

        public void SetPropertyValue(To value)
        {
            Debug.LogWarning($"[UIBinder] You cannot write to a modified property! {typeof(From).Name} -> {typeof(To).Name}");
        }

        public void ExecuteCommand(object value)
        {
            Debug.LogWarning($"[UIBinder] You cannot execute a command on a modified property! {typeof(From).Name} -> {typeof(To).Name}");
            return;
        }

        public IDisposable SubscribeList(Action<ListChangeEvent<To>> action)
        {
            Debug.LogWarning($"[UIBinder] You cannot subscribe to a list on a modified property! {typeof(From).Name} -> {typeof(To).Name}");
            return EmptyDisposable.Instance;
        }

        public void ApplyListChange(ListChangeEvent<To> @event)
        {
            Debug.LogWarning($"[UIBinder] You cannot apply a list change on a modified property! {typeof(From).Name} -> {typeof(To).Name}");
            return;
        }

        public string ToStringChain()
        {
            return $"ModifiedProperty<{typeof(From).Name}, {typeof(To).Name}> -> {_origin.ToStringChain()}";
        }
    }
}