using System;

using HUtil.Runtime.Command;
using HUtil.Runtime.Observable;

namespace HUtil.UI
{
    public interface IViewModelProperty
    {
        /// <summary>
        /// 프로퍼티를 ObservableProperty<T>로 변환합니다
        /// </summary>
        /// <typeparam name="T">변환할 타입</typeparam>
        /// <returns>ObservableProperty<T></returns>
        public ObservableProperty<T> AsObservableProperty<T>();

        /// <summary>
        /// 프로퍼티를 ObservableTrigger로 변환합니다
        /// </summary>
        /// <returns>ObservableTrigger</returns>
        public ObservableTrigger AsObservableTrigger();

        /// <summary>
        /// 프로퍼티를 CommandBase로 변환합니다
        /// </summary>
        /// <returns>CommandBase</returns>
        public CommandBase AsCommand();

        /// <summary>
        /// 프로퍼티를 ObservableList<T>로 변환합니다
        /// </summary>
        /// <typeparam name="T">변환할 타입</typeparam>
        /// <returns>ObservableList<T></returns>
        public ObservableList<T> AsObservableList<T>();
    }
}