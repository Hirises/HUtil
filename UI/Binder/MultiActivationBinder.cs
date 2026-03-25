using System;
using System.Collections.Generic;

using HUtil.Runtime;
using HUtil.Runtime.Extension;
using HUtil.Runtime.Observable;

using Sirenix.OdinInspector;

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
        [InfoBox("evaluateMultipleStates: true라면 모든 상태를 1회씩 체크합니다 (서로 중복되는 요소가 있을 경우 예기치 못한 동작이 발생할 수 있습니다)")]
        [SerializeField] private bool evaluateMultipleStates = true;

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
                bool isMatch = state._comparisonOperator.Compare(index, state._index);
                if(!evaluateMultipleStates && !isMatch){
                    continue;
                }
                foreach (var obj in state._onObjects)
                {
                    obj.SetActive(isMatch);
                }
            }
        }

        [Serializable]
        private class ActivationState{
            [SerializeField] public int _index;
            [SerializeField] public ComparisonOperator _comparisonOperator;
            [SerializeField] public GameObject[] _onObjects;
        }
    }
}