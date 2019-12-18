using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

namespace TSKT.Localizations
{
    [CreateAssetMenu(fileName = "TableAsset", menuName = "TSKT/Localization Table", order = 1023)]
    public class TableAsset : ScriptableObject
    {
        [System.Serializable]
        public class Sheet
        {
            [System.Serializable]
            public class Item
            {
                [System.Serializable]
                public class Pair
                {
                    public string language;
                    public string text;
                }

                public string key;
                public List<Pair> pairs = new List<Pair>();
            }

            public List<Item> items = new List<Item>();

            public void Merge(Sheet source)
            {
                foreach (var sourceItem in source.items)
                {
                    var item = items.FirstOrDefault(_ => _.key == sourceItem.key);
                    if (item == null)
                    {
                        item = new Item
                        {
                            key = sourceItem.key
                        };
                        items.Add(item);
                    }

                    foreach (var sourcePair in sourceItem.pairs)
                    {
                        var pair = item.pairs.FirstOrDefault(_ => _.language == sourcePair.language);
                        if (pair == null)
                        {
                            pair = new Item.Pair
                            {
                                language = sourcePair.language
                            };
                            item.pairs.Add(pair);
                        }
                        else
                        {
                            Debug.Log("conflict : " + sourceItem.key + ", " + sourcePair.language + ", [" + pair.text + " and " + sourcePair.text + "]");
                        }
                        pair.text = sourcePair.text;
                    }
                }
            }
            public Dictionary<string, Dictionary<string, string>> CreateLanguageKeyTextDictionary()
            {
                var languageKeyValues = new Dictionary<string, Dictionary<string, string>>();
                foreach (var item in items)
                {
                    foreach (var languageValue in item.pairs)
                    {
                        var language = languageValue.language;
                        var value = languageValue.text;
                        if (!languageKeyValues.TryGetValue(language, out var dict))
                        {
                            dict = new Dictionary<string, string>();
                            languageKeyValues.Add(language, dict);
                        }
                        dict.Add(item.key, value);
                    }
                }
                return languageKeyValues;
            }
        }

        [System.Serializable]
        class Language
        {
            public SystemLanguage code;
            public string[] words;
        }

        [SerializeField]
        string[] sortedKeys = default;

        [SerializeField]
        Language[] languages = default;

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

        public SystemLanguage[] Languages
        {
            get
            {
                return languages.Select(_ => _.code).ToArray();
            }
        }

#if UNITY_STANDALONE || UNITY_EDITOR

        public void Build()
        {
            var mergedSheet = new Sheet();

            var folder = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Localization");
            var pathes = Directory.GetFiles(folder, "*.json", SearchOption.TopDirectoryOnly);

            foreach (var path in pathes)
            {
                var jsonString = File.ReadAllText(path);

                var sheet = JsonUtility.FromJson<Sheet>(jsonString);
                Debug.Assert(sheet != null, "json parse error : " + jsonString);

                if (sheet != null)
                {
                    mergedSheet.Merge(sheet);
                }
            }

            var languageKeyWords = new DoubleDictionary<SystemLanguage, string, string>();
            foreach (var item in mergedSheet.items)
            {
                var japaneseText = item.pairs.FirstOrDefault(_ => _.language == SystemLanguage.Japanese.ToString())?.text;
                foreach (var pair in item.pairs)
                {
                    if (System.Enum.TryParse<SystemLanguage>(pair.language, out var lang))
                    {
                        if (System.Array.IndexOf(LocalizationSetting.Instance.Languages, lang) < 0)
                        {
                            continue;
                        }
                        languageKeyWords.Add(lang, item.key, pair.text);
                    }
                }
            }

            sortedKeys = mergedSheet.items
                .Select(_ => _.key)
                .Distinct()
                .ToArray();
            System.Array.Sort(sortedKeys, System.StringComparer.Ordinal);

            var languages = new List<Language>();

            foreach (var (languageCode, key, word) in languageKeyWords)
            {
                var language = languages.FirstOrDefault(_ => _.code == languageCode);
                if (language == null)
                {
                    language = new Language
                    {
                        code = languageCode,
                        words = new string[sortedKeys.Length]
                    };
                    languages.Add(language);
                }

                var i = System.Array.IndexOf(sortedKeys, key);

                Debug.Assert(language.words[i] == null, "duplicated key : " + languageCode + ", " + key);
                language.words[i] = word;
            }
            this.languages = languages.ToArray();
        }
#endif

#if UNITY_EDITOR
        void OnValidate()
        {
            Build();
            UnityEditor.EditorUtility.SetDirty(this);
        }

        public int JapaneseTotalLength
        {
            get
            {
                return languages.First(_ => _.code == SystemLanguage.Japanese)
                    .words
                    .Where(_ => !string.IsNullOrEmpty(_))
                    .Select(_ => _.Length)
                    .Sum();
            }
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

        public void Verify()
        {
            var japanese = languages.First(_ => _.code == SystemLanguage.Japanese);

            foreach (var language in languages)
            {
                if (language.code == SystemLanguage.Japanese)
                {
                    continue;
                }

                for (int i=0; i<sortedKeys.Length; ++i)
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

        public void GenerateCode()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendLine("namespace TSKT");
            builder.AppendLine("{");

            builder.AppendLine("    public static class TableKey");
            builder.AppendLine("    {");
            for (int index = 0; index < sortedKeys.Length; ++index)
            {
                var identifier = sortedKeys[index]
                    .Replace('.', '_')
                    .Replace("%", "percent")
                    .Replace("+", "plus")
                    .Replace("\'", "quotation")
                    .Replace("?", "question")
                    .Replace("？", "question");
                builder.AppendLine("        public const int " +  identifier + " = " + index.ToString() + ";");
            }
            builder.AppendLine("    }");
            builder.AppendLine("}");

            System.IO.File.WriteAllText("Assets/Generated/TableKey.cs", builder.ToString());
        }
#endif
    }
}
