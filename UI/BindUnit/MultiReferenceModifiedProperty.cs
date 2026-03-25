using System;
using System.Collections.Generic;
using System.Linq;

using HUtil.Runtime.Extension;
using HUtil.Runtime.Observable;

using UnityEngine;

namespace HUtil.UI
{
    public struct MultiReferenceModifiedProperty<From, To> : IViewModelProperty<To>
    {
        private List<IViewModelProperty<From>> _references;
        private Func<To> _getter;

        public MultiReferenceModifiedProperty(List<IViewModelProperty<From>> references, Func<To> getter)
        {
            _references = references ?? throw new ArgumentNullException(nameof(references));
            _getter = getter ?? throw new ArgumentNullException(nameof(getter));
        }

        public IDisposable SubscribeProperty(Action<To> action)
        {
            var getter = _getter;
            var disposable = new CompositeDisposable();
            foreach(var reference in _references){
                reference.SubscribeProperty(value => action(getter())).AddTo(disposable);
            }
            return disposable;
        }

        public To GetPropertyValue()
        {
            return _getter();
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
            return $"MultiReferenceModifiedProperty<{typeof(From).Name}, {typeof(To).Name}> -> {string.Join(", ", _references.Select(reference => reference.ToStringChain()))}";
        }
    }
}