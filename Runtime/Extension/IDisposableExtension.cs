using UnityEngine;
using System;
using HUtil.Runtime.Observable;

namespace HUtil.Runtime.Extension
{
    /// <summary>
    /// <see cref="IDisposable"/> 확장 메서드
    /// </summary>
    public static class IDisposableExtension
    {
        /// <summary>
        /// <see cref="IDisposable"/>를 <see cref="CompositeDisposable"/>에 추가합니다
        /// </summary>
        /// <param name="disposable">추가할 <see cref="IDisposable"/></param>
        /// <param name="compositeDisposable">추가할 <see cref="CompositeDisposable"/></param>
        /// <returns><see cref="CompositeDisposable"/></returns>
        public static CompositeDisposable AddTo(this IDisposable disposable, CompositeDisposable compositeDisposable)
        {
            compositeDisposable.Add(disposable);
            return compositeDisposable;
        }
    }
}
