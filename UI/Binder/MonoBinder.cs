using System;
using System.Collections.Generic;
using System.Linq;

using HUtil.Runtime.Observable;

using UnityEngine;

namespace HUtil.UI.Binder
{
    /// <summary>
    /// BindMap을 받고, 수정하고, 전파하는 주체
    /// </summary>
    public abstract class MonoBinder : MonoBehaviour
    {
        [SerializeField] private MonoBinder _parentBinder = null;
        [SerializeField] private List<MonoBinder> _childBinders = new List<MonoBinder>();
        /// <summary>
        /// 이 바인더의 상위 바인더
        /// </summary>
        internal MonoBinder ParentBinder => _parentBinder;
        /// <summary>
        /// 이 바인더가 관리하는 하위 바인더들
        /// </summary>
        public IReadOnlyList<MonoBinder> ChildBinders => _childBinders;

        /// <summary>
        /// 다른 바인더에게 바인딩 정보를 제공할지 여부
        /// </summary>
        protected virtual bool IsRootBinder => false;
        /// <summary>
        /// RootBinder에 대해서만, 바인딩 정보를 하위로 전파할지 여부
        /// </summary>
        protected virtual bool PropagateBinding => true;

        /// <summary>
        /// 바인딩 과정에서 발생하는 구독을 안전하게 해제하기 위한 <see cref="CompositeDisposable"/>
        /// </summary>
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        protected virtual void Reset()
        {
            UpdateBinderList();
        }

#region Update Binder List
        /// <summary>
        /// 바인더 참조 리스트를 업데이트 합니다. 연관된 Binder들도 자동으로 수정합니다
        /// </summary>
        public void UpdateBinderList()
        {
            UpdateParentBinderInternal();
            if(_parentBinder != null){
                _parentBinder.UpdateChildBindersInternal();
            }

            UpdateChildBindersInternal();
            foreach(var childBinder in _childBinders){
                childBinder.UpdateParentBinderInternal();
            }
        }

        private void UpdateParentBinderInternal()
        {
            _parentBinder = FindParentBinder();
        }

        private void UpdateChildBindersInternal()
        {
            FindChildBinders(_childBinders);
        }
#endregion

#region Find Binder Hierarchy
        /// <summary>
        /// 이 바인더가 관리할 책임이 있는 하위 바인더들을 찾습니다
        /// </summary>
        /// <param name="output">하위 바인더들을 저장할 리스트</param>
        /// <returns>하위 바인더들</returns>
        public List<MonoBinder> FindChildBinders(List<MonoBinder> output = null)
        {
            output ??= new List<MonoBinder>();
            output.Clear();

            if(!IsRootBinder){
                return output;
            }
    
            int selfIndex = gameObject.GetComponentIndex(this);
            for(int i = selfIndex + 1; i < gameObject.GetComponentCount(); i++){
                var comp = gameObject.GetComponentAtIndex(i);
                if(comp is MonoBinder binder){
                    output.Add(binder);
                    if(binder.IsRootBinder){
                        //하위로 전파하는 객체면 여기까지
                        return output;
                    }
                }
            }
            foreach(Transform child in transform){
                //자식들을 재귀적으로 검색
                FindChildBindersInternal(child, output);
            }
            return output;
        }
    
        private void FindChildBindersInternal(Transform self, List<MonoBinder> childBinders)
        {
            for(int i = 0; i < self.gameObject.GetComponentCount(); i++){
                var comp = self.gameObject.GetComponentAtIndex(i);
                if(comp is MonoBinder binder){
                    childBinders.Add(binder);
                    if(binder.IsRootBinder){
                        //하위로 전파하는 객체면 여기까지
                        return;
                    }
                }
            }
            foreach(Transform child in self.transform){
                //자식들을 재귀적으로 검색
                FindChildBindersInternal(child, childBinders);
            }
        }
    
        /// <summary>
        /// 이 바인더를 관리할 책임이 있는 상위 바인더를 찾습니다
        /// </summary>
        /// <returns></returns>
        public MonoBinder FindParentBinder()
        {
            foreach(var binder in EnumerateParentBinders()){
                if(binder.IsRootBinder){
                    return binder;
                }
            }
            return null;
        }
    
        /// <summary>
        /// 상위 바인더들을 순차적으로 검색합니다
        /// </summary>
        /// <returns>상위 바인더</returns>
        private IEnumerable<MonoBinder> EnumerateParentBinders()
        {
            int selfIndex = gameObject.GetComponentIndex(this);
            for(int i = selfIndex - 1; i >= 0; i--){
                var comp = gameObject.GetComponentAtIndex(i);
                if(comp is MonoBinder binder){
                    yield return binder;
                }
            }
            var curObject = transform.parent;
            while(curObject != null){
                for(int i = curObject.gameObject.GetComponentCount() - 1; i >= 0; i--){
                    var comp = curObject.gameObject.GetComponentAtIndex(i);
                    if(comp is MonoBinder binder){
                        yield return binder;
                    }
                }
                curObject = curObject.parent;
            }
        }
#endregion

#region Bind & Unbind
        /// <summary>
        /// 뷰모델을 바인딩합니다
        /// </summary>
        /// <param name="bindMap">바인딩할 뷰모델</param>
        internal void Bind(Dictionary<string, IViewModelProperty> bindMap)
        {
            if (bindMap == null)
            {
                BindingContext.LogWarning($"BindMap is null", gameObject);
                return;
            }
    
            BindingContext.LogDebug($"Bind", gameObject);
    
            Unbind();
            BindInternal(bindMap, _disposable);
            if(IsRootBinder && PropagateBinding){   //하위로 전파하는 객체면 하위 바인더들에게 전파
                BeforePropagate(bindMap, _disposable);
                foreach(var childBinder in _childBinders){
                    childBinder.Bind(bindMap);
                }
                AfterPropagate(bindMap, _disposable);
            }
        }
    
        /// <summary>
        /// 내부 바인딩 로직을 수행합니다
        /// <code>
        /// protected override void BindInternal(object viewModel, CompositeDisposable disposable)
        /// {
        ///     var prop = ReflectionHelper.GetObservableProperty&lt;string&gt;(viewModel, _propertyName);
        ///     prop.Subscribe(OnValueChanged).AddTo(disposable);
        /// }
        /// </code>
        /// </summary>
        /// <param name="bindMap">바인딩할 뷰모델</param>
        /// <param name="disposable">바인딩 과정에서 발생하는 구독을 등록하기 위한 <see cref="CompositeDisposable"/><br />이 객체에 등록된 구독은 <see cref="Unbind"/>에서 자동으로 해제됩니다</param>
        protected abstract void BindInternal(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable);

        /// <summary>
        /// 하위로 전파하기 전에 호출됩니다
        /// </summary>
        /// <param name="bindMap">바인딩할 뷰모델</param>
        protected virtual void BeforePropagate(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable)
        {
            //pass
        }

        /// <summary>
        /// 하위로 전파한 후에 호출됩니다
        /// </summary>
        /// <param name="bindMap">바인딩할 뷰모델</param>
        protected virtual void AfterPropagate(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable)
        {
            //pass
        }
    
        /// <summary>
        /// 바인딩된 뷰모델을 해제합니다
        /// </summary>
        public void Unbind()
        {
            if(IsRootBinder && PropagateBinding){   //하위로 전파하는 객체면 하위 바인더들에게 전파
                foreach(var childBinder in _childBinders){
                    childBinder.Unbind();
                }
            }
            UnbindInternal();
        }
    
        /// <summary>
        /// 내부 바인딩 해제 로직을 수행합니다 (본인만 신경쓰면 됩니다)
        /// </summary>
        protected virtual void UnbindInternal()
        {
            _disposable.Clear();
        }
    
        protected virtual void OnDestroy()
        {
            //컴포넌트가 파괴될 때 구독 해제
            _disposable.Dispose();
        }
#endregion
    
        /// <summary>
        /// 이 바인더가 제공하는 모든 바인딩 정보를 가져옵니다
        /// </summary>
        /// <returns>바인딩 정보 리스트</returns>
        internal virtual List<BindingInfo> GetAllBindingInfosEditor()
        {
            return _parentBinder?.GetAllBindingInfosEditor() ?? new List<BindingInfo>();
        }

        /// <summary>
        /// 이 바인더가 제공하는 모든 프로퍼티 중 주어진 타입과 방향으로 바인딩 가능한 프로퍼티의 이름을 가져옵니다
        /// </summary>
        /// <param name="receivingType">받을 수 있는 타입</param>
        /// <param name="bindingMode">동기화 하려는 방향</param>
        /// <returns>프로퍼티 이름 리스트</returns>
        internal List<string> GetAllBindablePropertyNamesEditor(BindingType receivingType, BindingMode bindingMode)
        {
            return GetAllBindingInfosEditor().Where(info => info.CanAccept(receivingType, bindingMode)).Select(info => info.PropertyPath).ToList();
        }
    }
}
