using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSKT
{
    public class LocalizationSetting : ScriptableObject
    {
        static LocalizationSetting instance;
        static public LocalizationSetting Instance
        {
            get
            {
                return instance ?? (instance = Resources.Load<LocalizationSetting>("LocalizationSetting"));
            }
        }

        [SerializeField]
        SystemLanguage[] languages = new[]
        {
            SystemLanguage.Japanese,
            SystemLanguage.English,
            SystemLanguage.ChineseTraditional,
            SystemLanguage.ChineseSimplified,
        };

        public SystemLanguage[] Languages => languages;

#if UNITY_EDITOR
        [UnityEditor.MenuItem("TSKT/Create Localization Setting")]
        static void CreateScriptableObject()
        {
            var obj = CreateInstance<LocalizationSetting>();
            UnityEditor.AssetDatabase.CreateAsset(obj, "Assets/Resources/LocalizationSetting.asset");
            UnityEditor.EditorUtility.SetDirty(obj);
        }
#endif
    }
}