using System;
using System.Collections.Generic;

using HUtil.Runtime.Observable;

using UnityEngine;

namespace HUtil.UI.Binder
{
    /// <summary>
    /// On/Off 그룹 바인더
    /// </summary>
    public class MultiActivationBinder : MonoBinder
    {
        [SerializeField] private ActivationState[] _activationStates;
        [SerializeField] private PropertyBindingPort<int> _index_prop = new PropertyBindingPort<int>(BindingType.OfType(BindingBaseType.Int), BindingDirectionFlags.ToUI);

        protected void OnValidate()
        {
            _index_prop.Validate(this);
        }

        protected override void BindInternal(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable)
        {
            _index_prop.Bind(bindMap, disposable, SetIndex);
        }

        private void SetIndex(int index)
        {
            foreach (var state in _activationStates)
            {
                foreach (var obj in state._onObjects)
                {
                    obj.SetActive(state._index == index);
                }
            }
        }

        [Serializable]
        private class ActivationState{
            [SerializeField] public int _index;
            [SerializeField] public GameObject[] _onObjects;
        }
    }
}