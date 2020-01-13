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

        public string Get(SystemLanguage language, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return key;
            }

            var wordIndex = System.Array.BinarySearch(sortedKeys, key, System.StringComparer.Ordinal);
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

        public void Verify(SystemLanguage defaultLanauage)
        {
            var japanese = languages.First(_ => _.code == defaultLanauage);

            foreach (var language in languages)
            {
                if (language.code == defaultLanauage)
                {
                    continue;
                }

                for (int i = 0; i < sortedKeys.Length; ++i)
                {
                    // 日本語が設定されていないなら他の言語に値があろうがなかろうがエラーではない
                    var jp = japanese.words[i];
                    if (string.IsNullOrEmpty(jp))
                    {
                        continue;
                    }

                    // 日本語が設定されているのに他言語が設定されていない->エラーになるべき
                    var word = language.words[i];
                    if (string.IsNullOrEmpty(word))
                    {
                        Debug.Log("翻訳不足 : " + language.code + ", " + sortedKeys[i]);
                    }
                }
            }
        }
    }
}
