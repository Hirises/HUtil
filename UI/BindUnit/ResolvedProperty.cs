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
        private IViewModel _viewModel;
        private string _path;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="viewModel">뷰모델 객체</param>
        /// <param name="path">프로퍼티 경로</param>
        public ResolvedProperty(IViewModel viewModel, string path){
            _viewModel = viewModel;
            _path = path;
        }

        /// <summary>
        /// 프로퍼티를 ObservableProperty<T>로 변환합니다
        /// </summary>
        /// <typeparam name="T">변환할 타입</typeparam>
        /// <returns>ObservableProperty<T></returns>
        public object AsObservableProperty<T>(){
            return UIRuntimeReflectionHelper.GetObservableProperty<T>(_viewModel, _path);
        }

        /// <summary>
        /// 프로퍼티를 ObservableTrigger로 변환합니다
        /// </summary>
        /// <returns>ObservableTrigger</returns>
        public ObservableTrigger AsObservableTrigger(){
            return UIRuntimeReflectionHelper.GetObservableTrigger(_viewModel, _path);
        }

        /// <summary>
        /// 프로퍼티를 CommandBase로 변환합니다
        /// </summary>
        /// <returns>CommandBase</returns>
        public CommandBase AsCommand(){
            return UIRuntimeReflectionHelper.GetCommand(_viewModel, _path);
        }

        /// <summary>
        /// 프로퍼티를 ObservableList<T>로 변환합니다
        /// </summary>
        /// <typeparam name="T">변환할 타입</typeparam>
        /// <returns>ObservableList<T></returns>
        public object AsObservableList<T>(){
            return UIRuntimeReflectionHelper.GetObservableList<T>(_viewModel, _path);
        }
    }
}