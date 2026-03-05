using UnityEngine;

using HUtil.UI.Binder;
using System.Collections.Generic;
using Unity.Collections;
using System;
using HUtil.Runtime.Observable;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HUtil.Editor")]
namespace HUtil.UI
{
    /// <summary>
    /// 재사용 가능한 UI 오브젝트의 단위
    /// </summary>
    public class UIComponent : MonoBinder
    {
        [SerializeField] private string _viewModelType;
        [SerializeField, ReadOnly] private List<MonoBinder> _binders = new List<MonoBinder>();

        internal string ViewModelType => _viewModelType;

        protected override void Reset()
        {
            UpdateBinderList();
            base.Reset();   //UIComponent의 경우 본인의 바인더 리스트 업데이트 후 부모 클래스의 Reset을 호출해야 함.
        }

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

        protected override void BindInternal(object viewModel, CompositeDisposable disposable)
        {
            foreach (var binder in _binders)
            {
                binder.Bind(viewModel);
            }
        }

        public override void Unbind()
        {
            foreach (var binder in _binders)
            {
                binder.Unbind();
            }
        }
    }
}
