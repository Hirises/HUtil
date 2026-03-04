using System;
using System.Threading;
using System.Threading.Tasks;

using HUtil.Runtime.Observable;

using Unity.Properties;

namespace HUtil.Runtime.Command
{
    /// <summary>
    /// 취소 가능한 비동기 명령을 실행하는 클래스
    /// </summary>
    [GeneratePropertyBag]
    public class CancelableCommand : VoidCommandBase
    {
        private Func<CancellationToken, Task> _execute;
        [CreateProperty]
        private ObservableProperty<bool> _canExecute;
        [CreateProperty]
        private ObservableProperty<bool> _isExecuting;
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// 명령 실행 가능 여부
        /// </summary>
        public override IReadOnlyObservableProperty<bool> CanExecute => _canExecute;
        /// <summary>
        /// 명령 실행 중 여부
        /// </summary>
        public IReadOnlyObservableProperty<bool> IsExecuting => _isExecuting;
        /// <summary>
        /// 명령 실행 취소 토큰
        /// </summary>
        public CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;

        /// <summary>
        /// 취소 가능한 비동기 명령을 생성합니다
        /// </summary>
        /// <param name="execute">명령 실행 함수</param>
        /// <param name="canExecute">명령 실행 가능 여부; null인 경우 항상 실행 가능</param>
        public CancelableCommand(Func<CancellationToken, Task> execute, ObservableProperty<bool> canExecute = null){
            _execute = execute;
            _canExecute = canExecute;
            _isExecuting = new(false);
            _cancellationTokenSource = null;
        }

        public override async void Execute(){
            if(_isExecuting.Value)
            {
                Cancel();
            }
            else
            {
                await ExecuteAsync();
            }
        }

        /// <summary>
        /// 명령을 실행합니다
        /// </summary>
        /// <returns>명령 실행 결과</returns>
        public async Task ExecuteAsync(){
            if(!_canExecute.Value){
                return;
            }
            if(_isExecuting.Value){
                return;
            }
            
            _cancellationTokenSource = new CancellationTokenSource();
            _isExecuting.Value = true;

            try
            {
                await _execute.Invoke(_cancellationTokenSource.Token);
            }
            finally
            {
                _isExecuting.Value = false;
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        /// <summary>
        /// 명령 실행을 취소합니다
        /// </summary>
        public void Cancel(){
            if(_cancellationTokenSource == null){
                return;
            }
            _cancellationTokenSource?.Cancel();
        }
    }

    /// <summary>
    /// 취소 가능한 비동기 명령을 실행하는 클래스
    /// </summary>
    /// <typeparam name="T">명령을 실행할 매개변수의 타입</typeparam>   
    [GeneratePropertyBag]
    public class CancelableCommand<T> : GenericCommandBase<T>
    {
        private Func<T, CancellationToken, Task> _execute;
        [CreateProperty]
        private ObservableProperty<bool> _canExecute;
        [CreateProperty]
        private ObservableProperty<bool> _isExecuting;
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// 명령 실행 가능 여부
        /// </summary>
        public override IReadOnlyObservableProperty<bool> CanExecute => _canExecute;
        /// <summary>
        /// 명령 실행 중 여부
        /// </summary>
        public IReadOnlyObservableProperty<bool> IsExecuting => _isExecuting;
        /// <summary>
        /// 명령 실행 취소 토큰
        /// </summary>
        public CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;

        /// <summary>
        /// 취소 가능한 비동기 명령을 생성합니다
        /// </summary>
        /// <param name="execute">명령 실행 함수</param>
        /// <param name="canExecute">명령 실행 가능 여부; null인 경우 항상 실행 가능</param>
        public CancelableCommand(Func<T, CancellationToken, Task> execute, ObservableProperty<bool> canExecute = null){
            _execute = execute;
            _canExecute = canExecute;
            _isExecuting = new(false);
            _cancellationTokenSource = null;
        }

        public override async void Execute(T parameter){
            if(_isExecuting.Value)
            {
                Cancel();
            }
            else
            {
                await ExecuteAsync(parameter);
            }
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
            if(_isExecuting.Value){
                return;
            }
            
            _cancellationTokenSource = new CancellationTokenSource();
            _isExecuting.Value = true;

            try
            {
                await _execute.Invoke(parameter, _cancellationTokenSource.Token);
            }
            finally
            {
                _isExecuting.Value = false;
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        /// <summary>
        /// 명령 실행을 취소합니다
        /// </summary>
        public void Cancel(){
            if(_cancellationTokenSource == null){
                return;
            }
            _cancellationTokenSource?.Cancel();
        }
    }
}