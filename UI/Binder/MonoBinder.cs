using System;
using System.Collections.Generic;

using HUtil.Runtime.Observable;

using UnityEngine;

namespace HUtil.UI.Binder
{
    /// <summary>
    /// ViewModel을 UI요소에 바인딩하는 기본 컴포넌트
    /// </summary>
    public abstract class MonoBinder : MonoBehaviour
    {
        /// <summary>
        /// 바인딩 과정에서 발생하는 구독을 안전하게 해제하기 위한 <see cref="CompositeDisposable"/>
        /// </summary>
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        protected virtual void Reset()
        {
            var uiComp = FindUIComponent();
            if (uiComp != null){
                uiComp.UpdateBinderList();    //바인더 리스트를 업데이트 할 책임은 UIComponent가 가짐. MonoBinder는 단순히 '업데이트 해줘!'하고 요청만 한다.
            }
        }

        /// <summary>
        /// 이 바인더가 속한 UIComponent를 찾습니다
        /// </summary>
        /// <returns>찾은 UIComponent, 없으면 null</returns>
        public UIComponent FindUIComponent()
        {
            if(transform.parent == null) return null;
            return transform.parent.GetComponentInParent<UIComponent>();
        }

        /// <summary>
        /// 뷰모델을 바인딩합니다
        /// </summary>
        /// <param name="bindMap">바인딩할 뷰모델</param>
        internal void Bind(Dictionary<string, ViewModelProperty> bindMap)
        {
            if (bindMap == null)
            {
                throw new ArgumentNullException(nameof(bindMap));
            }

            Unbind();
            BindInternal(bindMap, _disposable);
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
        protected abstract void BindInternal(Dictionary<string, ViewModelProperty> bindMap, CompositeDisposable disposable);

        /// <summary>
        /// 바인딩된 뷰모델을 해제합니다
        /// </summary>
        public virtual void Unbind()
        {
            _disposable.Clear();
        }

        protected virtual void OnDestroy()
        {
            //컴포넌트가 파괴될 때 구독 해제
            _disposable.Dispose();
        }
    }
}
