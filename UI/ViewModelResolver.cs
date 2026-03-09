using System;
using System.Collections.Generic;
using System.Linq;

using HUtil.Runtime.Extension;
using HUtil.Runtime.Observable;

using Unity.Properties;

using UnityEngine;

namespace HUtil.UI
{
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
        [SerializeField] private BindingMethod _bindingMethod;
        [SerializeField] private PropertyBindingInfo _viewModelProp;
        [SerializeField] private ViewModelBindingItem[] _bindMap;
        [SerializeField, HideInInspector] private UIComponent _parent;

        private object _viewModel;
        private IDisposable _subscription;

        public bool IsResolved => _viewModel != null;
        public string ViewModelType => _viewModelType;

        public ViewModelResolver(UIComponent parent){
            _bindingMethod = BindingMethod.ManualBinding;
            _viewModelProp = new PropertyBindingInfo(BindingType.ViewModel, BindDirectionFlags.ToUI);
            _viewModelType = string.Empty;
            _bindMap = new ViewModelBindingItem[0];
            _viewModel = null;
            _subscription = null;
            _parent = parent;
        }

        #region Bind ViewModel
        internal void OnEnable(){
            if(_bindingMethod != BindingMethod.StaticBinding){
                return;
            }
            _subscription = BindingContext.Subscribe(_viewModelType, SetViewModel);
        }

        internal void OnDisable(){
            _subscription?.Dispose();
            _subscription = null;
        }

        internal void DynamicBind(Dictionary<string, ViewModelProperty> bindMap, CompositeDisposable disposable){
            if(_bindingMethod != BindingMethod.DynamicBinding){
                return;
            }
            _viewModelProp.Bind<IViewModel>(bindMap, disposable, SetViewModel);
        }

        internal void ManualBind(object viewModel){
            if(_bindingMethod != BindingMethod.ManualBinding){  //이거 지울까?
                return;
            }
            if(viewModel.GetType().FullName != _viewModelType){
                return;
            }
            SetViewModel(viewModel);
        }

        internal void ManualUnbind(){
            if(_bindingMethod != BindingMethod.ManualBinding){  //이거 지울까?
                return;
            }
            SetViewModel(null);
        }

        private void SetViewModel(object viewModel){
            _viewModel = viewModel;
            _parent?.UpdateBindingState();
        }
        #endregion

        internal void Resolve(Dictionary<string, ViewModelProperty> bindMap){
            if(!IsResolved){
                return;
            }
            foreach(var bindInfo in _bindMap){
                bindMap.Add(bindInfo.DestinationPropertyPath, new ViewModelProperty(_viewModel, bindInfo.SourcePropertyPath));
            }
        }

        internal string ResolveName(string sourcePropertyPath){
            foreach(var bindInfo in _bindMap){
                if(bindInfo.SourcePropertyPath == sourcePropertyPath){
                    return bindInfo.DestinationPropertyPath;
                }
            }
            return sourcePropertyPath;
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
