using System;

using HUtil.Runtime.Observable;

namespace HUtil.Runtime.Command
{
    /// <summary>
    /// 실행가능한 명령의 기본 추상 클래스
    /// </summary>
    public abstract class CommandBase
    {
        /// <summary>
        /// 명령 실행 가능 여부
        /// </summary>
        public abstract IReadOnlyObservableProperty<bool> CanExecute { get; }

        /// <summary>
        /// 명령 실행
        /// </summary>
        /// <param name="parameter">매개변수</param>
        public abstract void Execute(object parameter);
    }
    
    /// <summary>
    /// 매개변수 없는 실행가능한 명령의 기본 추상 클래스
    /// </summary>
    public abstract class VoidCommandBase : CommandBase
    {
        public override sealed void Execute(object parameter)
        {
            Execute();
        }

        /// <summary>
        /// 명령 실행
        /// </summary>
        public abstract void Execute();
    }

    /// <summary>
    /// 매개변수가 있는 실행가능한 명령의 기본 추상 클래스
    /// </summary>
    /// <typeparam name="T">매개변수 타입</typeparam>
    public abstract class GenericCommandBase<T> : CommandBase
    {
        public override sealed void Execute(object parameter){
            Execute((T)parameter);
        }

        /// <summary>
        /// 명령 실행
        /// </summary>
        /// <param name="parameter">매개변수</param>
        public abstract void Execute(T parameter);
    }
}