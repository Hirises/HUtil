using System;

using HUtil.Runtime.Command;
using HUtil.Runtime.Observable;

namespace HUtil.UI
{
    public class ViewModelProperty
    {
        private object _viewModel;
        private string _path;

        public ViewModelProperty(object viewModel, string path){
            _viewModel = viewModel;
            _path = path;
        }

        public ObservableProperty<T> AsObservableProperty<T>(){
            return BinderReflectionHelper.GetObservableProperty<T>(_viewModel, _path);
        }

        public ObservableTrigger AsObservableTrigger(){
            return BinderReflectionHelper.GetObservableTrigger(_viewModel, _path);
        }

        public CommandBase AsCommand(){
            return BinderReflectionHelper.GetCommand(_viewModel, _path);
        }
    }
}