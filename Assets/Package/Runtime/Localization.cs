using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TSKT.Localizations;

namespace TSKT
{
    public class Localization
    {
        static Localization instance;

        Table table;

        SystemLanguage currentLanguage;
        static public SystemLanguage CurrentLanguage
        {
            get => instance.currentLanguage;
            set
            {
                instance.currentLanguage = value;
            }
        }

        Localization()
        {
        }

        public static void Create(SystemLanguage language)
        {
            instance = new Localization
            {
                currentLanguage = language
            };
        }

        public static void SetTable(Table table)
        {
            instance.table = table;
        }

        static public string Get(int key)
        {
            return Get(CurrentLanguage, key);
        }
        static public string Get(SystemLanguage language, int key)
        {
            return instance.table.Get(language, key);
        }

        static public string Get(string key)
        {
            return Get(CurrentLanguage, key);
        }

        static public string Get(SystemLanguage language, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return key;
            }

            return instance.table.Get(language, key);
        }

        static public string Get(int key, params (string key, LocalizationKey value)[] args)
        {
            return Get(CurrentLanguage, key, args);
        }

        static public string Get(SystemLanguage language, int key, params (string key, LocalizationKey value)[] args)
        {
            var value = Get(language, key);
            for (int i = 0; i < args.Length; ++i)
            {
                Debug.Assert(value.Contains(args[i].key),
                    value + "に" + args[i].key + "が見つかりません");
                value = value.Replace(args[i].key, args[i].value.Localize());
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
            for(int i=0; i<args.Length; ++i)
            {
                Debug.Assert(value.Contains(args[i].key),
                    value + "に" + args[i].key + "が見つかりません");
                value = value.Replace(args[i].key, args[i].value.Localize());
            }
            return value;
        }

        static public int GetIndex(string key)
        {
            return instance.table.GetIndex(key);
        }

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

        public static SystemLanguage[] Languages => instance?.table?.Languages;
        public static bool Contains(SystemLanguage language)
        {
            var langs = Languages;
            if (langs == null)
            {
                return false;
            }
            return System.Array.IndexOf(langs, language) > 0;
        }
    }
}
