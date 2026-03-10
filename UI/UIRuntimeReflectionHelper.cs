using UnityEngine;
using System;
using HUtil.Runtime.Observable;
using Unity.Properties;
using HUtil.Runtime.Command;
using System.Collections.Generic;
using System.Reflection;
using HUtil.Runtime.Extension;

namespace HUtil.UI
{
    /// <summary>
    /// UI 런타임 리플렉션 관련 확장 메서드
    /// </summary>
    internal static class UIRuntimeReflectionHelper
    {
        //property bag을 이용해서 런타임에 필드를 조회하는 메소드들
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
        /// 주어진 객체 내부의 <see cref="ObservableTrigger"/>를 가져옵니다.
        /// </summary>
        /// <param name="obj">객체</param>
        /// <param name="propertyName">프로퍼티 이름</param>
        /// <returns><see cref="ObservableTrigger"/></returns>
        public static ObservableTrigger GetObservableTrigger(object obj, string propertyName)
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
            if (PropertyContainer.TryGetValue(ref _obj, propertyName, out ObservableTrigger value))
            {
                return value;
            }
            else
            {
                throw new ArgumentException($"Trigger {propertyName} not found on object {obj.GetType().Name}");
            }
        }

        /// <summary>
        /// 주어진 객체 내부의 <see cref="CommandBase"/>를 가져옵니다.
        /// </summary>
        /// <param name="obj">객체</param>
        /// <param name="commandPath">필드 경로</param>
        /// <returns><see cref="CommandBase"/></returns>
        public static CommandBase GetCommand(object obj, string commandPath)
        {
            if (string.IsNullOrEmpty(commandPath))
            {
                throw new ArgumentNullException(nameof(commandPath), "CommandName is null or empty");
            }
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Object is null");
            }

            object _obj = obj;
            if (PropertyContainer.TryGetValue(ref _obj, commandPath, out CommandBase value))
            {
                return value;
            }
            else
            {
                throw new ArgumentException($"Command {commandPath} not found on object {obj.GetType().Name}");
            }
        }
        #endregion

        /// <summary>
        /// 주어진 필드의 바인딩 정보를 가져옵니다.
        /// </summary>
        /// <param name="field">필드</param>
        /// <returns>(바인딩 타입, 허용된 방향)</returns>
        public static BindingInfo GetBindingInfo(FieldInfo field)
        {
            //필드는 ObservableProperty<T>, ObservableTrigger, 또는 CommandBase를 상속하는 타입이어야 합니다.
            if(field.FieldType.IsSubclassOfGeneric(typeof(ObservableProperty<>)))    //ObservableProperty<T>
            {
                var underlyingType = field.FieldType.GetGenericArgumentsOfType(typeof(ObservableProperty<>))[0];  //제네릭 안쪽에 들어간 타입을 꺼내옴
                return new BindingInfo(field.Name, underlyingType.ToBindingType(), field.GetCustomAttribute<BindableAttribute>()?.AllowedDirection ?? BindingDirectionFlags.None);
            }
            else if(typeof(CommandBase).IsAssignableFrom(field.FieldType))    //CommandBase
            {
                return new BindingInfo(field.Name, BindingType.Command, BindingDirectionFlags.ToData);    //Command는 데이터로만 동기화 가능
            }
            else if(typeof(ObservableTrigger).IsAssignableFrom(field.FieldType))    //ObservableTrigger
            {
                return new BindingInfo(field.Name, BindingType.Trigger, field.GetCustomAttribute<BindableAttribute>()?.AllowedDirection ?? BindingDirectionFlags.None);
            }

            //지원하지 않는 타입
            return new BindingInfo(field.Name, BindingType.None, BindingDirectionFlags.None);
        }

        /// <summary>
        /// 주어진 ViewModel 타입 내부의 바인딩 가능한 모든 프로퍼티의 이름을 가져옵니다
        /// </summary>
        /// <param name="viewModelType">객체 타입</param>
        /// <param name="output">값을 전달받을 리스트</param>
        /// <returns>프로퍼티 이름 리스트</returns>
        public static List<string> GetAllBindablePropertyNames(Type viewModelType, List<string> output = null)
        {
            output ??= new();

            var fields = viewModelType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach(var field in fields){

                var bindingInfo = GetBindingInfo(field);
                if(!bindingInfo.IsValid){
                    continue;
                }

                //추가
                output.Add(field.Name);
            }

            return output;
        }

        /// <summary>
        /// 주어진 ViewModel 타입 내부의 바인딩 가능한 모든 프로퍼티의 바인딩 정보를 가져옵니다
        /// </summary>
        /// <param name="viewModelType">객체 타입</param>
        /// <param name="output">값을 전달받을 리스트</param>
        /// <returns>바인딩 정보 리스트</returns>
        public static List<BindingInfo> GetAllBindingInfos(Type viewModelType, List<BindingInfo> output = null)
        {
            output ??= new();

            var fields = viewModelType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach(var field in fields){
                var bindingInfo = GetBindingInfo(field);
                if(!bindingInfo.IsValid){
                    continue;
                }

                //추가
                output.Add(bindingInfo);
            }

            return output;
        }
    }
}
