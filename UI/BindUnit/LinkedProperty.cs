using System;

using HUtil.Runtime.Command;
using HUtil.Runtime.Extension;
using HUtil.Runtime.Observable;

namespace HUtil.UI
{
    public class VirtualProperty<U> : IViewModelProperty
    {
        private ObservableProperty<U> _property;

        public VirtualProperty(ObservableProperty<U> property)
        {
            _property = property;
        }

        public static VirtualProperty<U> Create<T, U>(IReadOnlyObservableProperty<T> property, Func<T, U> expression, CompositeDisposable disposable)
        {
            var prop = new ObservableProperty<U>();
            prop.FollowWithExpression(property, expression).AddTo(disposable);
            return new VirtualProperty<U>(prop);
        }

        public CommandBase AsCommand()
        {
            return null;
        }

        public object AsObservableList<T>()
        {
            return null;
        }

        public object AsObservableProperty<T>()
        {
            return _property;
        }

        public ObservableTrigger AsObservableTrigger()
        {
            return null;
        }
    }
}