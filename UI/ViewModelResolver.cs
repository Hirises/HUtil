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
        [SerializeField] private BindingMethod _bindingMethod;
        [SerializeField] private PropertyBindingInfo _viewModelProp;
        [SerializeField] private ViewModelBindingItem[] _bindMap;
        [SerializeField, HideInInspector] private UIComponent _parent;

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

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="parent">부모 UIComponent</param>
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
            //StaticBinding을 위한 구독처리
            if(_bindingMethod != BindingMethod.StaticBinding){
                return;
            }
            _subscription = BindingContext.Subscribe(_viewModelType, SetViewModel);
        }

        internal void OnDisable(){
            //구독 해제
            _subscription?.Dispose();
            _subscription = null;
        }

        internal void DynamicBind(Dictionary<string, ResolvedProperty> bindMap, CompositeDisposable disposable){
            if(_bindingMethod != BindingMethod.DynamicBinding){
                return;
            }
            if(IsResolved){
                return;
            }
            _viewModelProp.Bind<IViewModel>(bindMap, disposable, SetViewModel);
        }

        internal void ManualBind(IViewModel viewModel){
            if(_bindingMethod != BindingMethod.ManualBinding){
                return;
            }
            SetViewModel(viewModel);
        }

        internal void ManualUnbind(){
            if(_bindingMethod != BindingMethod.ManualBinding){
                return;
            }
            SetViewModel(null);
        }

        private void SetViewModel(IViewModel viewModel){
            if(viewModel != null && viewModel.GetType().FullName != _viewModelType){
                //타입이 불일치하면 return
                return;
            }
            _viewModel = viewModel;
            _parent?.UpdateBindingState();
        }
        #endregion

        /// <summary>
        /// 뷰모델을 해석하고, 바인딩 정보를 생성합니다
        /// </summary>
        /// <param name="bindMap">바인딩 정보를 저장할 딕셔너리</param>
        internal void Resolve(Dictionary<string, ResolvedProperty> bindMap){
            if(!IsResolved){
                return;
            }
            foreach(var bindInfo in _bindMap){
                bindMap.Add(bindInfo.DestinationPropertyPath, new ResolvedProperty(_viewModel, bindInfo.SourcePropertyPath));
            }
        }

        /// <summary>
        /// 외부 프로퍼티 경로를 내부 프로퍼티 경로로 변환합니다 <br />
        /// 만약 매핑 정보에 없는 경우 원본 경로를 반환합니다
        /// </summary>
        /// <param name="sourcePropertyPath">외부 프로퍼티 경로</param>
        /// <returns>내부 프로퍼티 경로</returns>
        internal string ConvertPropertyPath(string sourcePropertyPath){
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
