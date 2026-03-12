using UnityEngine;
using System;
using HUtil.UI.Binder;
using System.Collections.Generic;
using HUtil.Runtime.Observable;

namespace HUtil.UI
{
    /// <summary>
    /// 프로퍼티를 추가/수정하는 컴포넌트
    /// </summary>
    public class ConditionalConverter : MonoBinder
    {
        protected override bool IsRootBinder => true;

        protected override void BindInternal(Dictionary<string, ResolvedProperty> bindMap, CompositeDisposable disposable)
        {
            //pass
        }
    }
}
