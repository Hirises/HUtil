using System;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HUtil.Runtime.Extension
{
    /// <summary>
    /// 런타임에서 동작하는 리플렉션 관련 확장 메서드
    /// </summary>
    public static class RuntimeReflectionHelper
    {
        /// <summary>
        /// 이 타입이 주어진 제네릭 타입의 서브클래스인 경우, 선언된 제네릭 인수를 가져옵니다.
        /// </summary>
        /// <param name="toCheck">확인할 타입</param>
        /// <param name="generic">제네릭 타입</param>
        /// <returns>제네릭 인수</returns>
        public static Type[] GetGenericArgumentsOfType(this Type toCheck, Type generic){
            while (toCheck != null && toCheck != typeof(object))
            {
                // 현재 타입이 제네릭인지 확인하고, 원본 정의(Definition)를 가져옴
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return toCheck.GetGenericArguments();
                }
                // 부모 클래스로 올라가며 다시 체크 (상속 관계 대응)
                toCheck = toCheck.BaseType;
            }
            return new Type[0];
        }

        /// <summary>
        /// 이 타입이 주어진 제네릭 타입의 서브클래스인지 확인합니다.
        /// </summary>
        /// <param name="toCheck">확인할 타입</param>
        /// <param name="generic">제네릭 타입</param>
        /// <returns>서브클래스 여부</returns>
        public static bool IsSubclassOfGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                // 현재 타입이 제네릭인지 확인하고, 원본 정의(Definition)를 가져옴
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                // 부모 클래스로 올라가며 다시 체크 (상속 관계 대응)
                toCheck = toCheck.BaseType;
            }
            return false;
        }
        
        /// <summary>
        /// 모든 <paramref name="baseType"/> 타입 구현체를 가져옵니다.
        /// </summary>
        /// <param name="baseType">기반 타입</param>
        /// <returns>구현체 타입 리스트</returns>
        public static List<Type> GetAllConcreteTypesDerivedFrom(Type baseType)
        {
            #if UNITY_EDITOR
                // 1. TypeCache를 통해 IViewModel을 상속/구현한 모든 타입을 즉시 가져옵니다.
                var typeCollection = TypeCache.GetTypesDerivedFrom(baseType); 
                // 클래스인 경우: TypeCache.GetTypesDerivedFrom(typeof(ViewModelBase));

                // 2. 추상 클래스(Abstract)나 인터페이스 자체는 인스턴스화할 수 없으므로 제외합니다.
                var concreteTypes = typeCollection
                    .Where(t => !t.IsAbstract && !t.IsInterface)
                    .ToList();

                return concreteTypes;
            #else
                var results = new List<Type>();
                
                // 현재 도메인에 로드된 모든 어셈블리를 순회합니다.
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    // 유니티 시스템 어셈블리 등 불필요한 스캔을 방지하고 싶다면 여기서 필터링 가능합니다.
                    // if (!assembly.FullName.Contains("YourProjectNamespace")) continue;

                    try
                    {
                        var types = assembly.GetTypes()
                            .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);
                        
                        results.AddRange(types);
                    }
                    catch (ReflectionTypeLoadException)
                    {
                        // 어셈블리 로드 중 발생할 수 있는 예외 처리 (일부 DLL 로드 실패 시)
                        continue;
                    }
                }

                return results;
            #endif
        }
    }
}