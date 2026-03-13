using System;
using System.Collections.Generic;
using System.Linq;

using HUtil.Runtime.Extension;
using HUtil.Runtime.Observable;

using Sirenix.OdinInspector;

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
        [SerializeField, ValueDropdown(nameof(GetPossibleViewModelTypes)), OnValueChanged(nameof(OnViewModelTypeChanged))] private string _viewModelType;
        private List<string> GetPossibleViewModelTypes() => RuntimeReflectionHelper.GetAllConcreteTypesDerivedFrom(typeof(IViewModel)).Select(type => type.AssemblyQualifiedName).ToList();
        [SerializeField] private BindingMethod _bindingMethod = BindingMethod.ManualBinding;
        [SerializeField, ShowIf(nameof(_bindingMethod), BindingMethod.DynamicBinding)] 
        private PropertyBindingPort _viewModelProp = new PropertyBindingPort(BindingType.OfType(BindingBaseType.ViewModel), BindingDirectionFlags.ToUI);
        [SerializeField]
        //[TableList(AlwaysExpanded = true, HideToolbar = true, NumberOfItemsPerPage = 10, ShowPaging = true, ShowIndexLabels = false)]
        [ListDrawerSettings(DefaultExpandedState = true, ShowFoldout = false, ShowPaging = true, DraggableItems = false, HideAddButton = true, HideRemoveButton = true)]
         private ViewModelBindingItem[] _bindMap;
        private void OnViewModelTypeChanged(){
            var viewModelType = Type.GetType(_viewModelType);
            if(viewModelType == null){
                _bindMap = new ViewModelBindingItem[0];
                return;
            }
            _bindMap = UIRuntimeReflectionHelper.GetAllBindingInfo(viewModelType).Select(info => new ViewModelBindingItem(info.PropertyPath, info.PropertyPath)).ToArray();
        }
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
            _viewModelProp = new PropertyBindingPort(BindingType.OfType(BindingBaseType.ViewModel), BindingDirectionFlags.ToUI);
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
            BindingContext.LogDebug($"SubscribeStaticBind: {_viewModelType}", sender.gameObject);
            _subscription = BindingContext.Subscribe(_viewModelType, (viewModel) => SetViewModel(viewModel, sender));
        }

        internal void UnsubscribeStaticBind(){
            //구독 해제
            _subscription?.Dispose();
            _subscription = null;
        }

        internal void DynamicBind(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable, MonoResolver sender){
            if(_bindingMethod != BindingMethod.DynamicBinding){
                return;
            }
            BindingContext.LogDebug($"DynamicBind: {_viewModelType}", sender.gameObject);
            _viewModelProp.Bind<IViewModel>(bindMap, disposable, (viewModel) => SetViewModel(viewModel, sender));
        }

        internal void ManualBind(IViewModel viewModel, MonoResolver sender){
            if(_bindingMethod != BindingMethod.ManualBinding){
                return;
            }
            if(IsResolved){
                return;
            }
            SetViewModel(viewModel, sender);
        }

        internal void ManualUnbind(MonoResolver sender){
            if(_bindingMethod != BindingMethod.ManualBinding){
                return;
            }
            SetViewModel(null, sender);
        }

        private void SetViewModel(IViewModel viewModel, MonoResolver sender){
            BindingContext.LogDebug($"SetViewModel: {viewModel?.GetType().FullName}", sender.gameObject);
            _viewModel = viewModel;
            sender?.UpdateBindingState();
        }
        #endregion

        /// <summary>
        /// 뷰모델을 해석하고, 바인딩 정보를 생성합니다
        /// </summary>
        /// <param name="bindMap">바인딩 정보를 저장할 딕셔너리</param>
        internal void GenerateBindMap(Dictionary<string, IViewModelProperty> bindMap){
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
        internal List<BindingInfo> GetAllBindingInfosEditor(List<BindingInfo> output = null)
        {
            output ??= new();
            UIRuntimeReflectionHelper.GetAllBindingInfo(Type.GetType(_viewModelType), output);
            return output;
        }

        [Serializable]
        private class ViewModelBindingItem
        {
            [SerializeField, DisplayAsString, HorizontalGroup, HideLabel] private string _sourcePath;
            [SerializeField, HorizontalGroup, HideLabel] private string _destinationPath;

            public string SourcePropertyPath => _sourcePath;
            public string DestinationPropertyPath => _destinationPath;

            public ViewModelBindingItem(string sourcePath, string destinationPath){
                _sourcePath = sourcePath;
                _destinationPath = destinationPath;
            }
        }
    }
}
