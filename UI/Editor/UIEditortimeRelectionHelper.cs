using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using HUtil.Editor;
using HUtil.Runtime.Command;
using HUtil.Runtime.Extension;
using HUtil.Runtime.Observable;

namespace HUtil.UI.Editor
{
    /// <summary>
    /// UI 에디터 리플렉션 관련 헬퍼 클래스
    /// </summary>
    public static class UIEditortimeReflectionHelper
    {

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
            else if(field.FieldType.IsSubclassOfGeneric(typeof(ObservableList<>)))    //ObservableList<T>
            {
                return new BindingInfo(field.Name, BindingType.List, field.GetCustomAttribute<BindableAttribute>()?.AllowedDirection ?? BindingDirectionFlags.None);
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
        
        /// <summary>
        /// 주어진 ViewModelResolver의 바인딩 정보를 가져옵니다<br />
        /// 내부 이름으로 변환하여 반환합니다
        /// </summary>
        /// <param name="viewModelResolver">ViewModelResolver</param>
        /// <param name="output">값을 전달받을 리스트</param>
        /// <returns>바인딩 정보 리스트</returns>
        public static List<BindingInfo> GetAllResolvedBindingInfos(ViewModelResolver viewModelResolver, List<BindingInfo> output = null)
        {
            Type viewModelType = EditortimeReflectionHelper.GetAllConcreteTypesDerivedFrom(typeof(IViewModel)).FirstOrDefault(type => type.FullName == viewModelResolver.ViewModelType);
            var bindingInfos = GetAllBindingInfos(viewModelType);
            output ??= new();
            foreach(var bindingInfo in bindingInfos){
                var internalPropertyPath = viewModelResolver.ConvertPropertyPath(bindingInfo.PropertyPath);
                output.Add(new BindingInfo(internalPropertyPath, bindingInfo.Type, bindingInfo.AllowedDirection));
            }
            return output;
        }

        /// <summary>
        /// 주어진 MonoResolver에 등록된 모든 ViewModel 타입을 가져옵니다
        /// </summary>
        /// <param name="resolver">UIComponent</param>
        /// <returns>ViewModel 타입 리스트</returns>
        public static List<Type> GetAllViewModelTypes(MonoResolver resolver)
        {
            var typeNameSet = resolver.ViewModelResolvers.Select(vmr => vmr.ViewModelType).ToHashSet();
            return EditortimeReflectionHelper.GetAllConcreteTypesDerivedFrom(typeof(IViewModel)).Where(typeStr => typeNameSet.Contains(typeStr.FullName)).ToList();
        }

        /// <summary>
        /// 주어진 MonoResolver에 등록된 모든 바인딩 가능한 프로퍼티의 이름을 가져옵니다
        /// </summary>
        /// <param name="resolver">MonoResolver</param>
        /// <param name="receivingType">받는 타입</param>
        /// <param name="bindingMode">바인딩 모드</param>
        /// <returns>프로퍼티 이름 리스트</returns>
        public static List<string> GetAllBindablePropertyNames(MonoResolver uiComponent, BindingType receivingType, BindingMode bindingMode)
        {
            List<BindingInfo> output = new();
            foreach(var viewModelResolver in uiComponent.ViewModelResolvers){
                GetAllResolvedBindingInfos(viewModelResolver, output);
            }
            return output.Where(b => b.CanAccept(receivingType, bindingMode)).Select(b => b.PropertyPath).ToList();
        }
    }
}
