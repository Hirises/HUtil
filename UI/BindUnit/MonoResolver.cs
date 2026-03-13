using System;
using System.Collections.Generic;

using HUtil.Runtime.Observable;
using HUtil.UI.Binder;

using UnityEngine;

namespace HUtil.UI
{
    /// <summary>
    /// ViewModel을 관리하고, 바인딩을 처리하는 컴포넌트
    /// </summary>
    public class MonoResolver : MonoBinder
    {
        public override bool IsRootBinder => true;
        protected override bool PropagateBinding => false;

        [SerializeField] private List<ViewModelResolver> _viewModelResolvers = new List<ViewModelResolver>();
        internal List<ViewModelResolver> ViewModelResolvers => _viewModelResolvers;

        private bool _isBinded = false;


#region Binding Methods
        protected override void BindInternal(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable)
        {
            //상위 UIComponent에서 내려오는 요청은 본인의 DynamicBind로 처리 (내부 리로드는 실행하지 않음)
            foreach (var resolver in _viewModelResolvers)
            {
                resolver.DynamicBind(bindMap, disposable, this);
            }
        }
    
        private void Awake(){
            _isBinded = false;
            foreach (var resolver in _viewModelResolvers)
            {
                resolver.SubscribeStaticBind(this);
            }
        }
    
        protected override void OnDestroy(){
            foreach (var resolver in _viewModelResolvers)
            {
                resolver.UnsubscribeStaticBind();
            }
            base.OnDestroy();
        }
    
        /// <summary>
        /// ViewModel을 수동으로 바인딩합니다
        /// </summary>
        /// <param name="viewModel">바인딩할 ViewModel</param>
        public void ManualBind(IViewModel viewModel)
        {
            BindingContext.LogDebug($"ManualBind: {viewModel.GetType().FullName}", gameObject);
            foreach (var resolver in _viewModelResolvers)
            {
                resolver.ManualBind(viewModel, this);
            }
        }
    
        /// <summary>
        /// Resolver들의 바인딩 상태를 확인하고, 하위 Binder들에게 바인딩을 요청합니다
        /// </summary>
        internal void UpdateBindingState(){
            bool allResolved = true;
            foreach(var resolver in _viewModelResolvers){
                if(!resolver.IsResolved){
                    allResolved = false;
                    break;
                }
            }

            if(!allResolved && _isBinded){
                Unbind();
            }else if(allResolved && !_isBinded){
                Dictionary<string, IViewModelProperty> bindMap = new Dictionary<string, IViewModelProperty>();
                foreach (var resolver in _viewModelResolvers)
                {
                    resolver.GenerateBindMap(bindMap);
                }
                //propagate
                foreach(var childBinder in ChildBinders){
                    childBinder.Bind(bindMap);
                }
                _isBinded = true;
            }
        }

        protected override void UnbindInternal()
        {
            _isBinded = false;
            foreach(var childBinder in ChildBinders){
                childBinder.Unbind();
            }
            base.UnbindInternal();
        }
#endregion
    
        internal override Dictionary<string, BindingInfo> GetAllProvidingBindingInfosEditor()
        {
            //resolver들의 바인딩 정보를 가져옴
            Dictionary<string, BindingInfo> output = new();
            foreach(var resolver in _viewModelResolvers){
                foreach(var info in resolver.GetAllBindingInfosEditor()){
                    output.Add(info.PropertyPath, info);
                }
            }
            return output;
        }
    }
}