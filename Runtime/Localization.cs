using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TSKT.Localizations;

namespace TSKT
{
    public class Localization
    {
        public static Localization Instance { get; private set; }

        System.Func<TableAsset> tableLoadTask;
        TableAsset table;
        TableAsset Table
        {
            get
            {
                if (table == null)
                {
                    table = tableLoadTask();
                    tableLoadTask = null;
                }
                return table;
            }
        }

        SystemLanguage currentLanguage;
        static public SystemLanguage CurrentLanguage
        {
            get => Instance.currentLanguage;
            set
            {
                Instance.currentLanguage = value;
            }
        }

        Localization()
        {
        }

        public static void Create(System.Func<TableAsset> tableLoadTask, SystemLanguage language)
        {
            Instance = new Localization
            {
                tableLoadTask = tableLoadTask,
                currentLanguage = language
            };
        }

        static public string Get(int key)
        {
            return Instance.Table.Get(CurrentLanguage, key);
        }

        static public string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return key;
            }

            return Instance.Table.Get(CurrentLanguage, key);
        }

        static public string Get(int key, params (string key, string value)[] args)
        {
            var value = Get(key);
            for (int i = 0; i < args.Length; ++i)
            {
                Debug.Assert(value.Contains(args[i].key),
                    value + "に" + args[i].key + "が見つかりません");
                value = value.Replace(args[i].key, args[i].value);
            }
            return value;
        }

        static public string Get(string key, params (string key, string value)[] args)
        {
            var value = Get(key);
            for(int i=0; i<args.Length; ++i)
            {
                Debug.Assert(value.Contains(args[i].key),
                    value + "に" + args[i].key + "が見つかりません");
                value = value.Replace(args[i].key, args[i].value);
            }
            return value;
        }

        public static void ForceRefresh()
        {
            foreach(var it in Object.FindObjectsOfType<LocalizationLabel>())
            {
                it.Refresh();
            }
        }

        public static SystemLanguage[] Languages => Instance.Table.Languages;
    }
}
