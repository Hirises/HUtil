using UnityEngine;
using System;
using System.Collections.Generic;

namespace HUtil.Runtime.Observable
{
    /// <summary>
    /// 여러 <see cref="IDisposable"/>를 묶어서 관리하기 위한 클래스
    /// </summary>
    public class CompositeDisposable : IDisposable
    {
        private bool _isDisposed = false;
        /// <summary>
        /// 이 <see cref="CompositeDisposable"/>가 해제되었는지 여부
        /// </summary>
        public bool IsDisposed => _isDisposed;
        private readonly List<IDisposable> _disposables;

        /// <summary>
        /// 주어진 <see cref="IDisposable"/>들을 묶어서 <see cref="CompositeDisposable"/>를 생성합니다
        /// </summary>
        /// <param name="disposables">대상 <see cref="IDisposable"/>들</param>
        public CompositeDisposable(params IDisposable[] disposables)
        {
            _isDisposed = false;
            _disposables = new List<IDisposable>(disposables);
        }

        /// <summary>
        /// 주어진 <see cref="IDisposable"/>를 추가합니다
        /// </summary>
        /// <param name="disposable">대상 <see cref="IDisposable"/></param>
        public void Add(IDisposable disposable)
        {
            if (disposable == null)
            {
                return;
            }

            if (_isDisposed)
            {
                disposable.Dispose();
                return;
            }

            _disposables.Add(disposable);
        }

        /// <summary>
        /// 이 <see cref="CompositeDisposable"/>를 해제합니다
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            foreach (var disposable in _disposables)
            {
                disposable?.Dispose();
            }
            _disposables.Clear();
        }
    }
}
