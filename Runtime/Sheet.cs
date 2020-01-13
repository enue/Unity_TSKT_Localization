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

        DoubleDictionary<SystemLanguage, string, string> CreateLanguageKeyTextDictionary()
        {
            var languageKeyWords = new DoubleDictionary<SystemLanguage, string, string>();
            foreach (var item in items)
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
            return languageKeyWords;
        }

        public Table ToTable()
        {
            var sortedKeys = items
                .Select(_ => _.key)
                .Distinct()
                .ToArray();
            System.Array.Sort(sortedKeys, System.StringComparer.Ordinal);

            var languages = new List<Table.Language>();

            foreach (var (languageCode, key, word) in CreateLanguageKeyTextDictionary())
            {
                var language = languages.FirstOrDefault(_ => _.code == languageCode);
                if (language == null)
                {
                    language = new Table.Language
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

            return new Table(sortedKeys, languages.ToArray());
        }

        public static Sheet Create(DoubleDictionary<string, string, string> languageKeyTextDictionary)
        {
            var result = new Sheet();
            foreach(var (language, key, text) in languageKeyTextDictionary)
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
            var jsonStrings = new ArrayBuilder<string>(pathes.Length);
            foreach (var path in pathes)
            {
                Debug.Log(path);
                var jsonString = File.ReadAllText(path);
                jsonStrings.Add(jsonString);
            }
            return CreateFromJsonStrings(jsonStrings.Array);
        }

        static public Sheet CreateFromJsonStrings(params string[] jsonStrings)
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
#endif
    }
}
