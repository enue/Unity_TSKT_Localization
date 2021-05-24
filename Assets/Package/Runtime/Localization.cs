#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TSKT.Localizations;
#if TSKT_LOCALIZATION_SUPPORT_UNIRX
using UniRx;
#endif

namespace TSKT
{
    public class Localization
    {
        static Localization? instance;
        static Localization Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Localization();
                }
                return instance;
            }
        }

        Table? table;

#if TSKT_LOCALIZATION_SUPPORT_UNIRX
        public static readonly ReactiveProperty<SystemLanguage> currentLanguage = new ReactiveProperty<SystemLanguage>(default);
        static public SystemLanguage CurrentLanguage
        {
            get => currentLanguage.Value;
            set => currentLanguage.Value = value;
        }
#else
        SystemLanguage currentLanguage;
        static public SystemLanguage CurrentLanguage
        {
            get => Instance.currentLanguage;
            set => Instance.currentLanguage = value;
        }
#endif

        Localization()
        {
        }

        public static void SetTable(Table table)
        {
            Instance.table = table;
        }

        static public string? Get(int key)
        {
            return Get(CurrentLanguage, key);
        }
        static public string Get(SystemLanguage language, int key)
        {
            return instance?.table?.Get(language, key) ?? key.ToString();
        }

        static public string? Get(string key)
        {
            return Get(CurrentLanguage, key);
        }

        static public string Get(SystemLanguage language, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return key;
            }

            return instance?.table?.Get(language, key) ?? key;
        }

        static public string Get(int key, params (string key, LocalizationKey value)[] args)
        {
            return Get(CurrentLanguage, key, args);
        }

        static public string Get(SystemLanguage language, int key, params (string key, LocalizationKey value)[] args)
        {
            var value = Get(language, key);
            foreach (var (_key, _value) in args)
            {
                value = value.Replace(_key, _value.Localize());
            }
            return value;
        }

        static public string Get(string key, params (string key, LocalizationKey value)[] args)
        {
            return Get(CurrentLanguage, key, args);
        }

        static public string Get(SystemLanguage language, string key, params (string key, LocalizationKey value)[] args)
        {
            var value = Get(language, key);
            foreach (var (_key, _value) in args)
            {
                value = value.Replace(_key, _value.Localize());
            }
            return value;
        }

        static public int GetIndex(string key)
        {
            return instance?.table?.GetIndex(key) ?? -1;
        }

#if !TSKT_LOCALIZATION_SUPPORT_UNIRX
        public static void ForceRefresh()
        {
            foreach(var it in Object.FindObjectsOfType<LocalizationLabel>())
            {
                it.Refresh();
            }

            foreach (var it in Object.FindObjectsOfType<FontSelector>())
            {
                it.Refresh();
            }
    }
#endif

        public static SystemLanguage[]? Languages => instance?.table?.Languages;
        public static bool Contains(SystemLanguage language)
        {
            var langs = Languages;
            if (langs == null)
            {
                return false;
            }
            return System.Array.IndexOf(langs, language) >= 0;
        }

        public static SystemLanguage GetPreferredLanguageForDevice(SystemLanguage fallback)
        {
            if (Contains(Application.systemLanguage))
            {
                return Application.systemLanguage;
            }
            return fallback;
        }
    }
}
