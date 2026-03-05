using UnityEngine;
using System;
using HUtil.Runtime.Observable;
using Unity.Properties;
using HUtil.Runtime.Command;
using System.Collections.Generic;
using System.Reflection;
using HUtil.Runtime.Extension;

namespace HUtil.Runtime.UI
{
    /// <summary>
    /// 바인더 리플렉션 관련 확장 메서드
    /// </summary>
    internal static class BinderReflectionHelper
    {
        #region GetProp
        /// <summary>
        /// 주어진 객체 내부의 <see cref="ObservableProperty{T}"/>를 가져옵니다.
        /// </summary>
        /// <typeparam name="T">프로퍼티 타입</typeparam>
        /// <param name="obj">객체</param>
        /// <param name="propertyName">프로퍼티 이름</param>
        /// <returns><see cref="ObservableProperty{T}"/></returns>
        public static ObservableProperty<T> GetObservableProperty<T>(object obj, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName), "PropertyName is null or empty");
            }
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Object is null");
            }

            object _obj = obj;
            if (PropertyContainer.TryGetValue(ref _obj, propertyName, out ObservableProperty<T> value))
            {
                return value;
            }
            else
            {
                throw new ArgumentException($"Property {propertyName} not found on object {obj.GetType().Name}");
            }
        }

        /// <summary>
        /// 주어진 객체 내부의 <see cref="CommandBase"/>를 가져옵니다.
        /// </summary>
        /// <param name="obj">객체</param>
        /// <param name="commandName">명령 이름</param>
        /// <returns><see cref="CommandBase"/></returns>
        public static CommandBase GetCommand(object obj, string commandName)
        {
            if (string.IsNullOrEmpty(commandName))
            {
                throw new ArgumentNullException(nameof(commandName), "CommandName is null or empty");
            }
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Object is null");
            }

            object _obj = obj;
            if (PropertyContainer.TryGetValue(ref _obj, commandName, out CommandBase value))
            {
                return value;
            }
            else
            {
                throw new ArgumentException($"Command {commandName} not found on object {obj.GetType().Name}");
            }
        }
        #endregion

        /// <summary>
        /// 주어진 ViewModel 타입 내부의 바인딩 가능한 모든 프로퍼티의 이름을 가져옵니다
        /// </summary>
        /// <param name="viewModelType">객체 타입</param>
        /// <param name="receivingType">받을 수 있는 타입</param>
        /// <param name="bindMode">동기화 하려는 방향</param>
        /// <returns>프로퍼티 이름 리스트</returns>
        public static List<string> GetAllBindablePropertyNames(Type viewModelType, BindingType receivingType, BindingMode bindMode)
        {
            var propertyNames = new List<string>();

            var fields = viewModelType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            BindingType fieldType = BindingType.None;
            BindDirectionFlags allowedDirections = BindDirectionFlags.None;
            foreach(var field in fields){

                //필드는 ObservableProperty<T>, ObservableTrigger, 또는 CommandBase를 상속하는 타입이어야 합니다.
                if(field.FieldType.IsSubclassOfGeneric(typeof(ObservableProperty<>))){
                    var underlyingType = field.FieldType.GetGenericArguments(typeof(ObservableProperty<>))[0];
                    fieldType = underlyingType.ToBindingType();
                    allowedDirections = field.GetCustomAttribute<ViewModelValueAttribute>().SyncronizeDirection;
                }else if(field.FieldType.IsAssignableFrom(typeof(CommandBase))){
                    fieldType = BindingType.Command;
                    allowedDirections = BindDirectionFlags.ToData;    //Command는 데이터로만 동기화 가능
                }else if(field.FieldType.IsAssignableFrom(typeof(ObservableTrigger))){
                    fieldType = BindingType.Trigger;
                    allowedDirections = field.GetCustomAttribute<ViewModelValueAttribute>().SyncronizeDirection;
                }else{
                    continue;
                }

                //할당 가능한 타입 검사
                if(!receivingType.CanAccept(fieldType)){
                    continue;
                }
                //방향검사
                if(!allowedDirections.IsAllowed(bindMode)){
                    continue;
                }

                //추가
                propertyNames.Add(field.Name);
            }

            return propertyNames;
        }
    }
}
