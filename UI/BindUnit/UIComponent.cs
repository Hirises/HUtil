using UnityEngine;

using HUtil.UI.Binder;
using System.Collections.Generic;
using Unity.Collections;
using System;
using HUtil.Runtime.Observable;
using System.Runtime.CompilerServices;

namespace HUtil.UI
{
    /// <summary>
    /// 재사용 가능한 UI 오브젝트의 단위
    /// </summary>
    public class UIComponent : MonoBinder
    {
        public override bool IsRootBinder => true;

        public List<BindingInfo> bindingInfos = new List<BindingInfo>();

        protected override void BindInternal(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable)
        {
            //pass
        }

        internal override Dictionary<string, BindingInfo> GetAllProvidingBindingInfosEditor()
        {
            var infos = base.GetAllProvidingBindingInfosEditor();
            foreach(var info in bindingInfos)
            {
                infos[info.PropertyPath] = info;
            }
            return infos;
        }
    }
}
