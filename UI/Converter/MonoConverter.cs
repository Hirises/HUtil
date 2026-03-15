using System;
using System.Collections.Generic;

using HUtil.Runtime.Observable;
using HUtil.UI.Binder;

using UnityEngine;

namespace HUtil.UI.Converter
{
    /// <summary>
    /// 데이터를 수정하여 하위에 전달하는 컨버터 클래스
    /// </summary>
    public abstract class MonoConverter : MonoBinder
    {
        public override bool IsRootBinder => true;

        protected override void BindInternal(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable)
        {
            //pass
        }

        protected override void BeforePropagate(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable){
            OnConvertProperties(bindMap);
        }

        /// <summary>
        /// 속성을 변환합니다 <see cref="ConvertProperty"/>
        /// </summary>
        /// <param name="bindMap">바인딩 맵</param>
        protected abstract void OnConvertProperties(Dictionary<string, IViewModelProperty> bindMap);

        protected override void AfterPropagate(Dictionary<string, IViewModelProperty> bindMap, CompositeDisposable disposable){
            OnRestoreProperties(bindMap);
        }

        /// <summary>
        /// 속성을 복원합니다 <see cref="RestoreProperty"/>
        /// </summary>
        /// <param name="bindMap">바인딩 맵</param>
        protected abstract void OnRestoreProperties(Dictionary<string, IViewModelProperty> bindMap);

        internal override Dictionary<string, BindingInfo> GetAllProvidingBindingInfosEditor()
        {
            var bindingInfos = base.GetAllProvidingBindingInfosEditor();
            OnConvertBindingInfos(bindingInfos);
            return bindingInfos;
        }

        /// <summary>
        /// 바인딩 정보를 변환합니다 <see cref="ModifyBindingInfos"/> 
        /// </summary>
        /// <param name="bindingInfos">바인딩 정보</param>
        protected abstract void OnConvertBindingInfos(Dictionary<string, BindingInfo> bindingInfos);

        /// <summary>
        /// 속성을 변환합니다
        /// </summary>
        /// <typeparam name="From">변환할 속성의 타입</typeparam>
        /// <typeparam name="To">변환된 속성의 타입</typeparam>
        /// <param name="bindMap">바인딩 맵</param>
        /// <param name="fromPath">변환할 속성의 경로</param>
        /// <param name="toPath">변환된 속성의 경로</param>
        /// <param name="converter">변환 함수</param>
        /// <param name="previousProperty">이전 속성</param>
        protected void ConvertProperty<From, To>(Dictionary<string, IViewModelProperty> bindMap, string fromPath, string toPath, Func<From, To> converter, ref IViewModelProperty previousProperty){
            if(!bindMap.TryGetValue(fromPath, out var fromProperty) || !(fromProperty is IViewModelProperty<From> typedFromProperty)){
                BindingContext.LogWarning($"{fromPath} is not found", gameObject);
                return;
            }
            previousProperty = null;
            if(bindMap.TryGetValue(toPath, out var toProperty)){
                previousProperty = toProperty;
            }
            bindMap[toPath] = new ModiftedProperty<From, To>(typedFromProperty, converter);
        }

        /// <summary>
        /// 속성을 복원합니다
        /// </summary>
        /// <param name="bindMap">바인딩 맵</param>
        /// <param name="toPath">복원할 속성의 경로</param>
        /// <param name="previousProperty">이전 속성</param>
        protected void RestoreProperty(Dictionary<string, IViewModelProperty> bindMap, string toPath, ref IViewModelProperty previousProperty){
            if(previousProperty != null){
                bindMap[toPath] = previousProperty;
            }else{
                bindMap.Remove(toPath);
            }
        }
    }
}