#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace TSKT
{
    [CreateAssetMenu(fileName = "LocalizationSetting", menuName = "TSKT/Localization Setting", order = 1023)]
    public class LocalizationSetting : ScriptableObject
    {
        [SerializeField]
        SystemLanguage[] languages = new[]
        {
            SystemLanguage.Japanese,
            SystemLanguage.English,
            SystemLanguage.ChineseTraditional,
            SystemLanguage.ChineseSimplified,
        };

        public SystemLanguage[] Languages => languages;

        [SerializeField]
        string replacerRegex = "{[^:.-]*?}";
        public System.Text.RegularExpressions.Regex ReplacerRegex => new System.Text.RegularExpressions.Regex(replacerRegex);

        public static LocalizationSetting? Instance => AssetDatabase.FindAssets("t:LocalizationSetting")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<LocalizationSetting>)
            .DefaultIfEmpty(null)
            .FirstOrDefault();
    }
}
