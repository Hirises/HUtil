using System;

using HUtil.Runtime.Command;
using HUtil.Runtime.Observable;

namespace HUtil.UI
{
    public interface IViewModelProperty
    {
        void ExecuteCommand(object value);
    }

    public interface IViewModelProperty<T> : IViewModelProperty
    {
        IDisposable SubscribeProperty(Action<T> action);
        T GetPropertyValue();
        void SetPropertyValue(T value);
        IDisposable SubscribeList(Action<ListChangeEvent<T>> action);
        void ApplyListChange(ListChangeEvent<T> @event);
    }
}