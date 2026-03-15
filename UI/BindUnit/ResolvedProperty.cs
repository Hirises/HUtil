using System;

using HUtil.Runtime.Command;
using HUtil.Runtime.Observable;

namespace HUtil.UI
{
    /// <summary>
    /// UIResolver에서 해석한 뷰모델 프로퍼티 정보
    /// </summary>
    public struct ResolvedProperty<T> : IViewModelProperty<T>
    {
        //실제 프로퍼티 값 참조
        private object _value;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="value">프로퍼티 값</param>
        public ResolvedProperty(object value){
            _value = value;
        }

        /// <summary>
        /// 프로퍼티를 ObservableProperty<T>로 변환합니다
        /// </summary>
        /// <typeparam name="T">변환할 타입</typeparam>
        /// <returns>ObservableProperty<T></returns>
        private ObservableProperty<T> AsObservableProperty(){
            return _value as ObservableProperty<T>;
        }

        /// <summary>
        /// 프로퍼티를 CommandBase로 변환합니다
        /// </summary>
        /// <returns>CommandBase</returns>
        private CommandBase AsCommand(){
            return _value as CommandBase;
        }

        /// <summary>
        /// 프로퍼티를 ObservableList<T>로 변환합니다
        /// </summary>
        /// <typeparam name="T">변환할 타입</typeparam>
        /// <returns>ObservableList<T></returns>
        private ObservableList<T> AsObservableList(){
            return _value as ObservableList<T>;
        }

        public IDisposable SubscribeProperty(Action<T> action){
            return AsObservableProperty().Subscribe(action);
        }

        public T GetPropertyValue(){
            return AsObservableProperty().Value;
        }

        public void SetPropertyValue(T value)
        {
            AsObservableProperty().Value = value;
        }

        public void ExecuteCommand(object value){
            AsCommand().Execute(value);
        }

        public IDisposable SubscribeList(Action<ListChangeEvent<T>> action){
            return AsObservableList().Subscribe(action);
        }

        public void ApplyListChange(ListChangeEvent<T> @event){
            AsObservableList().ApplyChange(@event);
        }
    }
}