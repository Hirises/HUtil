using UnityEngine;
using System;
using HUtil.Runtime.Observable;

namespace HUtil.Runtime.Extension
{
    public static class IDisposableExtension
    {
        public static CompositeDisposable AddTo(this IDisposable disposable, CompositeDisposable compositeDisposable)
        {
            compositeDisposable.Add(disposable);
            return compositeDisposable;
        }
    }
}
