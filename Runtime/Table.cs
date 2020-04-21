using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TSKT.Localizations
{
    public class Table
    {
        [System.Serializable]
        public class Language
        {
            public SystemLanguage code;
            public string[] words;
        }

        public readonly string[] sortedKeys;
        public readonly Language[] languages;

        public Table(string[] sortedKeys, Language[] languages)
        {
            this.sortedKeys = sortedKeys;
            this.languages = languages;
        }

        public int GetIndex(string key)
        {
            return System.Array.BinarySearch(sortedKeys, key, System.StringComparer.Ordinal);
        }

        public string Get(SystemLanguage language, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return key;
            }

            var wordIndex = GetIndex(key);
            if (wordIndex < 0)
            {
                Debug.Assert(wordIndex >= 0, "not found translation : " + key + ", " + language);
                return key;
            }

            return Get(language, wordIndex);
        }

        public string Get(SystemLanguage language, int index)
        {
            foreach (var it in languages)
            {
                if (it.code == language)
                {
                    return it.words[index];
                }
            }
            Debug.Assert(false, "not found lang : " + language);
            return null;
        }

        public SystemLanguage[] Languages => languages.Select(_ => _.code).ToArray();

        public int GetTotalLength(SystemLanguage language)
        {
            return languages.First(_ => _.code == language)
                .words
                .Where(_ => !string.IsNullOrEmpty(_))
                .Select(_ => _.Length)
                .Sum();
        }

        public Dictionary<SystemLanguage, int> WordCountMap
        {
            get
            {
                return languages.ToDictionary(
                    _ => _.code,
                    _ => _.words.Count(word => !string.IsNullOrEmpty(word)));
            }
        }
    }
}
