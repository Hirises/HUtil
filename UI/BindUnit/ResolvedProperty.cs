using System;

using HUtil.Runtime.Command;
using HUtil.Runtime.Observable;

namespace HUtil.UI
{
    /// <summary>
    /// UIResolver에서 해석한 뷰모델 프로퍼티 정보
    /// </summary>
    public struct ResolvedProperty : IViewModelProperty
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
        public ObservableProperty<T> AsObservableProperty<T>(){
            return _value as ObservableProperty<T>;
        }

        /// <summary>
        /// 프로퍼티를 ObservableTrigger로 변환합니다
        /// </summary>
        /// <returns>ObservableTrigger</returns>
        public ObservableTrigger AsObservableTrigger(){
            return _value as ObservableTrigger;
        }

        /// <summary>
        /// 프로퍼티를 CommandBase로 변환합니다
        /// </summary>
        /// <returns>CommandBase</returns>
        public CommandBase AsCommand(){
            return _value as CommandBase;
        }

        /// <summary>
        /// 프로퍼티를 ObservableList<T>로 변환합니다
        /// </summary>
        /// <typeparam name="T">변환할 타입</typeparam>
        /// <returns>ObservableList<T></returns>
        public ObservableList<T> AsObservableList<T>(){
            return _value as ObservableList<T>;
        }

        public IDisposable SubscribeProperty<T>(Action<T> action){
            return AsObservableProperty<T>().Subscribe(action);
        }

        public T GetPropertyValue<T>(){
            return AsObservableProperty<T>().Value;
        }

        public void SetPropertyValue<T>(T value)
        {
            AsObservableProperty<T>().Value = value;
        }

        public void ExecuteCommand(object value){
            AsCommand().Execute(value);
        }

        public IDisposable SubscribeList<T>(Action<ListChangeEvent<T>> action){
            return AsObservableList<T>().Subscribe(action);
        }

        public void ApplyListChange<T>(ListChangeEvent<T> @event){
            AsObservableList<T>().ApplyChange(@event);
        }
    }
}