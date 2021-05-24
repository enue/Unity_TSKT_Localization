#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}