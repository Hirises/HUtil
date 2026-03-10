using System;

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
    }
}