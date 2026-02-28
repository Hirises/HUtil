using System;

using HUtil.Runtime.Observable;

using UnityEngine;

namespace HUtil.Runtime.UI.Binder
{
    /// <summary>
    /// Property를 Component에 할당해주는 최종 주체
    /// </summary>
    public abstract class MonoBinder : MonoBehaviour
    {
        /// <summary>
        /// 바인딩 과정에서 발생하는 구독을 안전하게 해제하기 위한 <see cref="CompositeDisposable"/>
        /// </summary>
        protected readonly CompositeDisposable _disposable = new CompositeDisposable();

        /// <summary>
        /// 뷰모델을 바인딩합니다
        /// </summary>
        /// <param name="viewModel">바인딩할 뷰모델</param>
        public abstract void Bind(object viewModel);

        protected virtual void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}
