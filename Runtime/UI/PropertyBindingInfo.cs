using System;

using HUtil.Runtime.Extension;
using HUtil.Runtime.Observable;

using UnityEngine;

namespace HUtil.Runtime.UI
{
    /// <summary>
    /// 프로퍼티 바인딩 인스펙터 속성
    /// </summary>
    [Serializable]
    public class PropertyBindingInfo
    {
        /// <summary>
        /// 바인딩할 프로퍼티의 이름입니다
        /// </summary>
        [SerializeField]
        public string Path { get; private set; }
        /// <summary>
        /// 바인딩할 방향입니다
        /// </summary>
        [SerializeField]
        public SyncronizeDirection Direction { get; private set; }


        /// <summary>
        /// 현재 설정에 맞춰서 바인딩을 진행합니다
        /// </summary>
        /// <typeparam name="T">대상 타입</typeparam>
        /// <param name="viewModel">뷰 모델 객체</param>
        /// <param name="disposable">구독 관리용 disposable</param>
        public void Bind<T>(object viewModel, CompositeDisposable disposable, Action<T> setter)
        {
            var observable = ReflectionHelper.GetObservableProperty<T>(viewModel, Path);
            switch (Direction)
            {
                case SyncronizeDirection.OneShotToTarget:
                {
                    setter(observable.Value);
                    break;
                }
                case SyncronizeDirection.OneWayToTarget:
                {
                    observable.Subscribe(setter).AddTo(disposable);
                    break;
                }
                case SyncronizeDirection.OneWayToSource:
                {

                    break;
                }
                case SyncronizeDirection.TwoWay:
                {
                    observable.Subscribe(setter).AddTo(disposable);
                    break;
                }
            }
        }
    }
}
