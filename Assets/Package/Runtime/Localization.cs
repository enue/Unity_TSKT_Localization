﻿#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TSKT.Localizations;
using R3;

namespace TSKT
{
    public class Localization
    {
        static Localization? instance;
        static Localization Instance => instance ??= new Localization();

        Table? table;

        public static readonly ReactiveProperty<SystemLanguage> currentLanguage = new(default);
        static public SystemLanguage CurrentLanguage
        {
            get => currentLanguage.Value;
            set => currentLanguage.Value = value;
        }

        Localization()
        {
        }

        public static void SetTable(Table table)
        {
            Instance.table = table;
        }

        public static string Get(int key)
        {
            return Get(CurrentLanguage, key);
        }
        public static string Get(SystemLanguage language, int key)
        {
            return instance?.table?.Get(language, key) ?? key.ToString();
        }

        public static string Get(string key)
        {
            return Get(CurrentLanguage, key);
        }

        public static string Get(SystemLanguage language, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return "";
            }

            return instance?.table?.Get(language, key) ?? key;
        }

        public static int GetIndex(string key)
        {
            return instance?.table?.GetIndex(key) ?? -1;
        }

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

        public static string ToOrdinal(string value)
        {
            var a = value.Length >= 2 ? value[^2] : '0';
            var b = value.Length >= 1 ? value[^1] : '0';
            return (a, b) switch
            {
                (not '1', '1') => value + "st",
                (not '1', '2') => value + "nd",
                (not '1', '3') => value + "rd",
                _ => value + "th",
            };
        }
    }
}
