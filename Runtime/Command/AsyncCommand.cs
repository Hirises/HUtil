using System;
using System.Threading.Tasks;

using HUtil.Runtime.Observable;

using Unity.Properties;

namespace HUtil.Runtime.Command
{
    /// <summary>
    /// 비동기 명령을 실행하는 클래스
    /// </summary>
    [GeneratePropertyBag]
    public class AsyncRelayCommand : VoidCommandBase
    {
        private Func<Task> _execute;
        [CreateProperty]
        private ObservableProperty<bool> _canExecute;
        private bool _allowMultipleExecution;
        [CreateProperty]
        private ObservableProperty<bool> _isExecuting;

        /// <summary>
        /// 명령 실행 가능 여부
        /// </summary>
        public override IReadOnlyObservableProperty<bool> CanExecute => _canExecute;
        /// <summary>
        /// 명령 실행 중 여부
        /// </summary>
        public IReadOnlyObservableProperty<bool> IsExecuting => _isExecuting;

        /// <summary>
        /// 비동기 명령을 생성합니다
        /// </summary>
        /// <param name="execute">명령 실행 함수</param>
        /// <param name="canExecute">명령 실행 가능 여부; null인 경우 항상 실행 가능</param>
        /// <param name="allowMultipleExecution">동시 실행 가능 여부</param>
        public AsyncRelayCommand(Func<Task> execute, ObservableProperty<bool> canExecute = null, bool allowMultipleExecution = false){
            _execute = execute;
            _canExecute = canExecute;
            _allowMultipleExecution = allowMultipleExecution;
            _isExecuting = new(false);
        }

        public override async void Execute(){
            await ExecuteAsync();
        }

        /// <summary>
        /// 명령을 실행합니다
        /// </summary>
        /// <returns>명령 실행 결과</returns>
        public async Task ExecuteAsync(){
            if(!_canExecute.Value){
                return;
            }

            if(!_allowMultipleExecution){
                if(_isExecuting){
                    return;
                }
                _isExecuting.Value = true;
            }

            try
            {
                await _execute.Invoke();
            }
            finally
            {
                _isExecuting.Value = false;
            }
        }
    }

    /// <summary>
    /// 비동기 명령을 실행하는 클래스
    /// </summary>
    /// <typeparam name="T">명령을 실행할 매개변수의 타입</typeparam>
    [GeneratePropertyBag]
    public class AsyncRelayCommand<T> : GenericCommandBase<T>
    {
        private Func<T, Task> _execute;
        [CreateProperty]
        private ObservableProperty<bool> _canExecute;
        private bool _allowMultipleExecution;
        [CreateProperty]
        private ObservableProperty<bool> _isExecuting;

        /// <summary>
        /// 명령 실행 가능 여부
        /// </summary>
        public override IReadOnlyObservableProperty<bool> CanExecute => _canExecute;
        /// <summary>
        /// 명령 실행 중 여부
        /// </summary>
        public IReadOnlyObservableProperty<bool> IsExecuting => _isExecuting;

        /// <summary>
        /// 비동기 명령을 생성합니다
        /// </summary>
        /// <param name="execute">명령 실행 함수</param>
        /// <param name="canExecute">명령 실행 가능 여부; null인 경우 항상 실행 가능</param>
        /// <param name="allowMultipleExecution">동시 실행 가능 여부</param>
        public AsyncRelayCommand(Func<T, Task> execute, ObservableProperty<bool> canExecute = null, bool allowMultipleExecution = false){
            _execute = execute;
            _canExecute = canExecute;
            _allowMultipleExecution = allowMultipleExecution;
            _isExecuting = new(false);
        }

        public override async void Execute(T parameter){
            await ExecuteAsync(parameter);
        }

        /// <summary>
        /// 명령을 실행합니다
        /// </summary>
        /// <param name="parameter">명령을 실행할 매개변수</param>
        /// <returns>명령 실행 결과</returns>
        public async Task ExecuteAsync(T parameter){
            if(!_canExecute.Value){
                return;
            }

            if(!_allowMultipleExecution){
                if(_isExecuting){
                    return;
                }
                _isExecuting.Value = true;
            }

            try
            {
                await _execute.Invoke(parameter);
            }
            finally
            {
                _isExecuting.Value = false;
            }
        }
    }
}