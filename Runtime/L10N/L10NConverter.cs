using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using Unity.Plastic.Newtonsoft.Json;

using UnityEngine;

namespace HUtil.Runtime.L10N
{
    /// <summary>
    /// Localization(L10N) 문자열 변환 클래스
    /// </summary>
    public class L10NConverter
    {
        private static Regex _keywordRegex = new Regex(@"{keyword=""(\w+)""}|{cardId=""(\w+)""}", RegexOptions.Compiled);
        private static Dictionary<string, string> _l10nDict = new Dictionary<string, string>();

        public static void SetLocale(string locale){
            UnloadAll();
            Load($"L10N/general_{locale}");
        }

        /// <summary>
        /// JSON 파일을 로드합니다.
        /// </summary>
        /// <param name="jsonPath">JSON 파일 경로</param>
        private static void Load(string jsonPath)
        {
            var json = Resources.Load<TextAsset>(jsonPath).text;
            _l10nDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);    //일단 단순한 key-value 쌍을 가정. 나중에 depth 만들거면 그때 수정하자
        }

        public static void UnloadAll()
        {
            _l10nDict.Clear();
        }

        /// <summary>
        /// 현재 locale에 따라 해당 키의 Localization(L10N) 문자열을 변환합니다
        /// </summary>
        /// <param name="key">변환할 키</param>
        /// <returns>변환된 문자열</returns>
        public static string Convert(string key)
        {
            if(_l10nDict.TryGetValue(key, out var value))
            {
                return value;
            }
            return key;
        }

        /// <summary>
        /// 전체 텍스트에서 키워드를 찾아서 변환합니다.
        /// </summary>
        /// <param name="rawString">변환할 문자열</param>
        /// <returns>변환된 문자열</returns>
        public static string ReplaceKey(string rawString){
            return _keywordRegex.Replace(rawString, match => Convert(match.Groups[1].Value));
        }
    }
}