using UnityEngine;

using HUtil.Runtime.UI.Binder;
using System.Collections.Generic;
using Unity.Collections;
using System;
using HUtil.Runtime.Observable;

namespace HUtil.Runtime.UI
{
    /// <summary>
    /// 재사용 가능한 UI 오브젝트의 단위
    /// </summary>
    public class ViewRoot : MonoBinder
    {
        [SerializeField, ReadOnly] private List<MonoBinder> _binders = new List<MonoBinder>();

        private void Reset()
        {
            FindBinders(_binders);
        }

        //하위의 모든 MonoBinder를 찾아서 리스트에 추가합니다
        private void FindBinders(List<MonoBinder> binders)
        {
            if (binders == null)
            {
                throw new ArgumentNullException(nameof(binders));
            }

            binders.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                FindBindersRecursive(child, binders);
            }
        }

        private void FindBindersRecursive(Transform parent, List<MonoBinder> binders)
        {
            if (parent == null) return;

            if (parent.gameObject.TryGetComponent<MonoBinder>(out var binder))
            {
                binders.Add(binder);
                if (binder is ViewRoot)
                {
                    return; //ViewRoot 하위 노드는 탐색에서 제외
                }
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                FindBindersRecursive(child, binders);
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
