using System;
using System.Collections.Generic;
using System.Diagnostics;

using HUtil.Runtime.Observable;

namespace HUtil.UI
{
    /// <summary>
    /// Static Binding을 제공하기 위한 싱글톤 컨텍스트
    /// </summary>
    public static class BindingContext
    {
        private static Dictionary<string, IViewModel> _viewModels = new Dictionary<string, IViewModel>();
        private static Dictionary<string, List<Action<IViewModel>>> _subscriptions = new Dictionary<string, List<Action<IViewModel>>>();

        /// <summary>
        /// 주어진 ViewModel 타입을 구독합니다<br />
        /// 콜벡의 파라미터는 nullable해야합니다 (unbinding시 null이 전달됨)
        /// </summary>
        /// <param name="viewModelType">구독할 ViewModel 타입</param>
        /// <param name="onViewModelChanged">ViewModel이 변경될 때 호출될 콜백 (nullable)</param>
        /// <returns>구독을 취소할 수 있는 IDisposable</returns>
        public static IDisposable Subscribe(string viewModelType, Action<IViewModel> onViewModelChanged){
            UnityEngine.Debug.Log($"Subscribe: {viewModelType}");

            if(_viewModels.TryGetValue(viewModelType, out var viewModel)){
                onViewModelChanged(viewModel);
            }

            if(!_subscriptions.TryGetValue(viewModelType, out var subscriptions)){
                subscriptions = new List<Action<IViewModel>>();
                _subscriptions[viewModelType] = subscriptions;
            }
            subscriptions.Add(onViewModelChanged);

            return new ScriptableDisposable(() => {
                if(_subscriptions.TryGetValue(viewModelType, out var subscriptions)){
                    subscriptions.Remove(onViewModelChanged);
                    if(subscriptions.Count == 0){
                        _subscriptions.Remove(viewModelType);
                    }
                }
            });
        }

        /// <summary>
        /// 주어진 ViewModel을 정적 바인딩에 등록합니다
        /// </summary>
        /// <param name="viewModel">등록할 ViewModel</param>
        public static void StaticBind(IViewModel viewModel){
            StaticUnbind(viewModel);

            var typeName = viewModel.GetType().FullName;
            _viewModels[typeName] = viewModel;
            UnityEngine.Debug.Log($"StaticBind: {typeName}");

            if(_subscriptions.TryGetValue(typeName, out var subscriptions)){
                foreach(var callback in subscriptions){
                    callback(viewModel);
                }
            }
        }

        /// <summary>
        /// 주어진 ViewModel을 정적 바인딩에서 제거합니다
        /// </summary>
        /// <param name="viewModel">제거할 ViewModel</param>
        public static void StaticUnbind(IViewModel viewModel){
            var typeName = viewModel.GetType().FullName;
            _viewModels.Remove(typeName);
            UnityEngine.Debug.Log($"StaticUnbind: {typeName}");

            if(_subscriptions.TryGetValue(typeName, out var subscriptions)){
                foreach(var callback in subscriptions){
                    callback(null);
                }
            }
        }
    }
}