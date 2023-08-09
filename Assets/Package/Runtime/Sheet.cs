#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

namespace TSKT.Localizations
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
                public string language = default!;
                public string text = default!;
            }

            public string key = default!;
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

        Dictionary<SystemLanguage, Dictionary<string, string>> CreateLanguageKeyTextDictionary(params SystemLanguage[] languages)
        {
            Debug.Assert(languages.Length > 0, "使う言語が何もしていされていません");
            var languageKeyWords = new Dictionary<SystemLanguage, Dictionary<string, string>>();

            foreach (var item in items)
            {
                foreach (var pair in item.pairs)
                {
                    if (System.Enum.TryParse<SystemLanguage>(pair.language, out var lang))
                    {
                        if (System.Array.IndexOf(languages, lang) < 0)
                        {
                            continue;
                        }
                        if (!languageKeyWords.TryGetValue(lang, out var dict))
                        {
                            dict =  new Dictionary<string, string>();
                            languageKeyWords.Add(lang, dict);
                        }
                        dict.Add(item.key, pair.text);
                    }
                }
            }
            return languageKeyWords;
        }

        public Table ToTable(params SystemLanguage[] languages)
        {
            Debug.Assert(languages.Length > 0, "使う言語が何もしていされていません");
            var sortedKeys = items
                .Select(_ => _.key)
                .Distinct()
                .ToArray();
            System.Array.Sort(sortedKeys, System.StringComparer.Ordinal);

            var langs = new List<Table.Language>();

            var languageKeyTexts = CreateLanguageKeyTextDictionary(languages);
            foreach (var languageCode in languages)
            {
                var language = new Table.Language
                {
                    code = languageCode,
                    words = new string[sortedKeys.Length]
                };
                langs.Add(language);
                if (languageKeyTexts.TryGetValue(languageCode, out var dict))
                {
                    foreach (var it in dict)
                    {
                        var i = System.Array.IndexOf(sortedKeys, it.Key);

                        Debug.Assert(language.words[i] == null, "duplicated key : " + languageCode + ", " + it.Key);
                        language.words[i] = System.Text.RegularExpressions.Regex.Unescape(it.Value);
                    }
                }
            }

            return new Table(sortedKeys, langs.ToArray());
        }

        static Sheet Create((string language, string key, string text)[] languageKeyTexts)
        {
            var result = new Sheet();
            foreach(var (language, key, text) in languageKeyTexts)
            {
                var item = result.items.FirstOrDefault(_ => _.key == key);
                if (item == null)
                {
                    item = new Item
                    {
                        key = key
                    };
                    result.items.Add(item);
                }
                var pair = new Item.Pair
                {
                    language = language,
                    text = text
                };
                item.pairs.Add(pair);
            }

            return result;
        }

#if UNITY_STANDALONE || UNITY_EDITOR
        static public Sheet CreateFromFolder(string folder)
        {
            var pathes = Directory.GetFiles(folder, "*.json", SearchOption.TopDirectoryOnly);
            return CreateFromFiles(pathes);
        }

        static public Sheet CreateFromFiles(params string[] pathes)
        {
            var jsonStrings = new List<string>(pathes.Length);
            foreach (var path in pathes)
            {
                Debug.Log(path);
                var jsonString = File.ReadAllText(path);
                jsonStrings.Add(jsonString);
            }
            return CreateFromJsonStrings(jsonStrings.ToArray());
        }
#endif

        public static Sheet CreateFromJsonStrings(params string[] jsonStrings)
        {
            return CreateFromJsonStrings(new System.ReadOnlySpan<string>(jsonStrings));
        }
        public static Sheet CreateFromJsonStrings(System.ReadOnlySpan<string> jsonStrings)
        {
            var mergedSheet = new Sheet();

            foreach (var jsonString in jsonStrings)
            {
                var sheet = JsonUtility.FromJson<Sheet>(jsonString);
                Debug.Assert(sheet != null, "json parse error : " + jsonString);

                if (sheet != null)
                {
                    mergedSheet.Merge(sheet);
                }
            }

            return mergedSheet;
        }

        public static Sheet FromSpreadsheet(string[][] cells)
        {
            var dict = new List<(string language, string key, string text)>();

            foreach (var row in cells.Skip(1))
            {
                var id = row[0];
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }
                for (int i = 1; i < row.Length; ++i)
                {
                    var cell = row[i];
                    var lang = cells[0][i];
                    dict.Add((lang, id, cell));
                }
            }

            return Create(dict.ToArray());
        }
    }
}
