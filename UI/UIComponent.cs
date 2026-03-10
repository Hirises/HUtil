using UnityEngine;

using HUtil.UI.Binder;
using System.Collections.Generic;
using Unity.Collections;
using System;
using HUtil.Runtime.Observable;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HUtil.UI.Editor")]
namespace HUtil.UI
{
    /// <summary>
    /// 재사용 가능한 UI 오브젝트의 단위
    /// </summary>
    public class UIComponent : MonoBinder
    {
        [SerializeField] private List<ViewModelResolver> _viewModelResolvers = new List<ViewModelResolver>();
        [SerializeField, ReadOnly] private List<MonoBinder> _binders = new List<MonoBinder>();

        internal List<ViewModelResolver> ViewModelResolvers => _viewModelResolvers;

        protected override void Reset()
        {
            UpdateBinderList();
            base.Reset();   //UIComponent의 경우 본인의 바인더 리스트 업데이트 후 부모 클래스의 Reset을 호출해야 함.
        }

#region [Internal] BinderList Management
        //하위의 모든 MonoBinder를 찾아서 리스트에 추가합니다
        internal void UpdateBinderList()
        {
            _binders.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                FindBindersRecursive(child);
            }
        }

        private void FindBindersRecursive(Transform parent)
        {
            if (parent == null) return;

            if (parent.gameObject.TryGetComponent<MonoBinder>(out var binder))
            {
                _binders.Add(binder);
                if (binder is UIComponent)
                {
                    return; //UIComponent 하위 노드는 탐색에서 제외
                }
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                FindBindersRecursive(child);
            }
        }
#endregion

#region [Internal] ViewModelResolver Management
        internal void AddNewViewModelResolver()
        {
            _viewModelResolvers.Add(new ViewModelResolver(this));
        }

        internal void RemoveViewModelResolver(int index)
        {
            _viewModelResolvers.RemoveAt(index);
        }
#endregion

        private void Awake(){
            foreach (var resolver in _viewModelResolvers)
            {
                resolver.OnEnable();
            }
        }

        protected override void OnDestroy(){
            foreach (var resolver in _viewModelResolvers)
            {
                resolver.OnDisable();
            }
            base.OnDestroy();
        }

        /// <summary>
        /// ViewModel을 수동으로 바인딩합니다
        /// </summary>
        /// <param name="viewModel">바인딩할 ViewModel</param>
        public void ManualBind(IViewModel viewModel)
        {
            foreach (var resolver in _viewModelResolvers)
            {
                resolver.ManualBind(viewModel);
            }
        }

        /// <summary>
        /// Resolver들의 바인딩 상태를 확인하고, 하위 Binder들에게 바인딩을 요청합니다
        /// </summary>
        internal void UpdateBindingState(){
            //전부 Resolve되었는지 확인
            bool isAllResolved = true;
            foreach (var resolver in _viewModelResolvers)
            {
                if(!resolver.IsResolved){
                    isAllResolved = false;
                    break;
                }
            }

            Debug.Log($"UpdateBindingState: {gameObject.name} {isAllResolved}");

            Unbind();   //중복 바인딩 방지
            if(isAllResolved){
                Dictionary<string, ResolvedProperty> bindMap = new Dictionary<string, ResolvedProperty>();
                foreach (var resolver in _viewModelResolvers)
                {
                    resolver.Resolve(bindMap);
                }
                foreach (var binder in _binders)
                {
                    binder.Bind(bindMap);
                }
            }
        }

        protected override void BindInternal(Dictionary<string, ResolvedProperty> bindMap, CompositeDisposable disposable)
        {
            //상위 UIComponent에서 내려오는 요청은 본인의 DynamicBind로 처리 (내부 리로드는 실행하지 않음)
            foreach (var resolver in _viewModelResolvers)
            {
                resolver.DynamicBind(bindMap, disposable);
            }
        }
    }
}
