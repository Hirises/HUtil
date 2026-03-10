using System;

namespace HUtil.Runtime.L10N
{
    /// <summary>
    /// Localization(L10N) 문자열 변환 클래스
    /// </summary>
    public class L10NConverter
    {
        /// <summary>
        /// 현재 locale에 따라 해당 키의 Localization(L10N) 문자열을 변환합니다
        /// </summary>
        /// <param name="key">변환할 키</param>
        /// <returns>변환된 문자열</returns>
        public static string Convert(string key)
        {
            return key;
        }
    }
}