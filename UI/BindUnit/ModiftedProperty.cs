using System;

using HUtil.Runtime.Observable;

using UnityEngine;

namespace HUtil.UI
{
    public struct ModiftedProperty<From, To> : IViewModelProperty
    {
        private IViewModelProperty _origin;
        private Func<From, To> _modifier;

        public ModiftedProperty(IViewModelProperty origin, Func<From, To> modifier)
        {
            _origin = origin ?? throw new ArgumentNullException(nameof(origin));
            _modifier = modifier ?? throw new ArgumentNullException(nameof(modifier));
        }

        public IDisposable SubscribeProperty<T>(Action<T> action)
        {
            var modifier = _modifier;
            return _origin.SubscribeProperty<From>(value => action((T)(object)modifier(value)));
        }

        public T GetPropertyValue<T>()
        {
            return (T)(object)_modifier(_origin.GetPropertyValue<From>());
        }

        public void SetPropertyValue<T>(T value)
        {
            Debug.LogWarning($"[UIBinder] You cannot write to a modified property! {typeof(From).Name} -> {typeof(To).Name}");
        }

        public void ExecuteCommand(object value)
        {
            _origin.ExecuteCommand(value);
        }

        public IDisposable SubscribeList<T>(Action<ListChangeEvent<T>> action)
        {
            return _origin.SubscribeList<T>(action);
        }

        public void ApplyListChange<T>(ListChangeEvent<T> @event)
        {
            _origin.ApplyListChange(@event);
        }
    }
}