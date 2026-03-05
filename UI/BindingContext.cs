using System;
using System.Collections.Generic;

using HUtil.Runtime.Observable;

namespace HUtil.UI
{
    public static class BindingContext
    {
        private static Dictionary<string, IViewModel> _viewModels = new Dictionary<string, IViewModel>();
        private static Dictionary<string, List<Action<IViewModel>>> _subscriptions = new Dictionary<string, List<Action<IViewModel>>>();

        public static IDisposable Subscribe(string viewModelType, Action<IViewModel> onViewModelChanged){
            if(_viewModels.TryGetValue(viewModelType, out var viewModel)){
                onViewModelChanged(viewModel);
            }

            if(!_subscriptions.TryGetValue(viewModelType, out var subscriptions)){
                subscriptions = new List<Action<IViewModel>>();
                _subscriptions[viewModelType] = subscriptions;
            }
            subscriptions.Add(onViewModelChanged);

            return new Subscription(() => _subscriptions[viewModelType].Remove(onViewModelChanged));
        }

        public static void Bind(IViewModel viewModel){
            var typeName = viewModel.GetType().Name;
            _viewModels[typeName] = viewModel;

            if(_subscriptions.TryGetValue(typeName, out var subscriptions)){
                foreach(var callback in subscriptions){
                    callback(viewModel);
                }
            }
        }

        //TODO UNBIND 기능 추가 필요
    }
}