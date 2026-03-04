using System;

using HUtil.Runtime.Observable;

namespace HUtil.Runtime.Command
{
    /// <summary>
    /// 명령을 실행하는 클래스
    /// </summary>
    public class RelayCommand : VoidCommandBase
    {
        private Action _execute;
        private ObservableProperty<bool> _canExecute;

        /// <summary>
        /// 명령 실행 가능 여부
        /// </summary>
        public override IReadOnlyObservableProperty<bool> CanExecute => _canExecute;

        /// <summary>
        /// 명령을 생성합니다
        /// </summary>
        /// <param name="execute">명령 실행 함수</param>
        /// <param name="canExecute">명령 실행 가능 여부; null인 경우 항상 실행 가능</param>
        public RelayCommand(Action execute, ObservableProperty<bool> canExecute = null){
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// 명령을 실행합니다
        /// </summary>
        public override void Execute(){
            _execute?.Invoke();
        }
    }

    /// <summary>
    /// 명령을 실행하는 클래스
    /// </summary>
    /// <typeparam name="T">명령을 실행할 매개변수의 타입</typeparam>
    public class RelayCommand<T> : GenericCommandBase<T>
    {
        private Action<T> _execute;
        private ObservableProperty<bool> _canExecute;

        /// <summary>
        /// 명령 실행 가능 여부
        /// </summary>
        public override IReadOnlyObservableProperty<bool> CanExecute => _canExecute;

        /// <summary>
        /// 명령을 생성합니다
        /// </summary>
        /// <param name="execute">명령 실행 함수</param>
        /// <param name="canExecute">명령 실행 가능 여부; null인 경우 항상 실행 가능</param>
        public RelayCommand(Action<T> execute, ObservableProperty<bool> canExecute = null){
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// 명령을 실행합니다
        /// </summary>
        /// <param name="parameter">명령을 실행할 매개변수</param>
        public override void Execute(T parameter){
            _execute?.Invoke(parameter);
        }
    }
}