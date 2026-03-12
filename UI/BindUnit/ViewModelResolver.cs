using System;
using System.Collections.Generic;
using System.Linq;

using HUtil.Runtime.Extension;
using HUtil.Runtime.Observable;

using Unity.Properties;

using UnityEngine;

namespace HUtil.UI
{
    /// <summary>
    /// <see cref="UIComponent"/>가 ViewModel 바인딩 정보를 관리하기 위한 내부 클래스
    /// </summary>
    [Serializable]
    public class ViewModelResolver
    {
        /// <summary>
        /// 뷰모델을 가져올 방법
        /// </summary>
        public enum BindingMethod{
            /// <summary>
            /// 코드상에서 직접 바인딩
            /// </summary>
            ManualBinding,
            /// <summary>
            /// 싱글톤 인스턴스를 가져옴
            /// </summary>
            StaticBinding,
            /// <summary>
            /// 인스펙터 상에서 동적으로 바인딩
            /// </summary>
            DynamicBinding,
        }
        [SerializeField] private string _viewModelType;
        [SerializeField] private BindingMethod _bindingMethod = BindingMethod.ManualBinding;
        [SerializeField] private PropertyBindingPort _viewModelProp = new PropertyBindingPort(BindingType.ViewModel, BindingDirectionFlags.ToUI);
        [SerializeField] private ViewModelBindingItem[] _bindMap;

        private IViewModel _viewModel;
        private IDisposable _subscription;

        /// <summary>
        /// 뷰모델이 바인딩 되어있는지 여부
        /// </summary>
        public bool IsResolved => _viewModel != null;
        /// <summary>
        /// 뷰모델의 타입
        /// </summary>
        public string ViewModelType => _viewModelType;

        public ViewModelResolver(){
            _bindingMethod = BindingMethod.ManualBinding;
            _viewModelProp = new PropertyBindingPort(BindingType.ViewModel, BindingDirectionFlags.ToUI);
            _viewModelType = string.Empty;
            _bindMap = new ViewModelBindingItem[0];
            _viewModel = null;
            _subscription = null;
        }

        #region Bind ViewModel
        internal void SubscribeStaticBind(MonoResolver sender){
            //StaticBinding을 위한 구독처리
            if(_bindingMethod != BindingMethod.StaticBinding){
                return;
            }
            _subscription = BindingContext.Subscribe(_viewModelType, (viewModel) => SetViewModel(viewModel, sender));
        }

        internal void UnsubscribeStaticBind(){
            //구독 해제
            _subscription?.Dispose();
            _subscription = null;
        }

        internal void DynamicBind(Dictionary<string, ResolvedProperty> bindMap, CompositeDisposable disposable, MonoResolver sender){
            if(_bindingMethod != BindingMethod.DynamicBinding){
                return;
            }
            _viewModelProp.Bind<IViewModel>(bindMap, disposable, (viewModel) => SetViewModel(viewModel, sender));
        }

        internal void ManualBind(IViewModel viewModel, MonoResolver sender){
            if(_bindingMethod != BindingMethod.ManualBinding){
                return;
            }
            ManualUnbind(sender);
            SetViewModel(viewModel, sender);
        }

        internal void ManualUnbind(MonoResolver sender){
            if(_bindingMethod != BindingMethod.ManualBinding){
                return;
            }
            SetViewModel(null, sender);
        }

        private void SetViewModel(IViewModel viewModel, MonoResolver sender){
            // if(viewModel != null && viewModel.GetType().FullName != _viewModelType){
            //     //타입이 불일치하면 return
            //     return;
            // }
            UnityEngine.Debug.Log($"SetViewModel: {viewModel?.GetType().FullName}");
            _viewModel = viewModel;
            sender?.UpdateBindingState();
        }
        #endregion

        /// <summary>
        /// 뷰모델을 해석하고, 바인딩 정보를 생성합니다
        /// </summary>
        /// <param name="bindMap">바인딩 정보를 저장할 딕셔너리</param>
        internal void GenerateBindMap(Dictionary<string, ResolvedProperty> bindMap){
            if(!IsResolved){
                return;
            }
            foreach(var bindInfo in _bindMap){
                bindMap.Add(bindInfo.DestinationPropertyPath, new ResolvedProperty(_viewModel, bindInfo.SourcePropertyPath));
            }
        }

        /// <summary>
        /// 이 ViewModelResolver가 제공하는 모든 바인딩 정보를 가져옵니다
        /// </summary>
        /// <param name="output">값을 전달받을 리스트</param>
        /// <returns>바인딩 정보 리스트</returns>
        internal List<BindingInfo> GetAllBindingInfos(List<BindingInfo> output = null)
        {
            output ??= new();
            UIRuntimeReflectionHelper.GetAllBindingInfo(Type.GetType(_viewModelType), output);
            return output;
        }

        // 딕셔너리 비주얼라이즈하기 귀찮아서 사용함
        [Serializable]
        private class ViewModelBindingItem
        {
            [SerializeField] private string _sourcePropertyPath;
            [SerializeField] private string _destinationPropertyPath;

            public string SourcePropertyPath => _sourcePropertyPath;
            public string DestinationPropertyPath => _destinationPropertyPath;

            public ViewModelBindingItem(string sourcePropertyPath, string destinationPropertyPath){
                _sourcePropertyPath = sourcePropertyPath;
                _destinationPropertyPath = destinationPropertyPath;
            }
        }
    }
}
