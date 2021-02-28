using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TSKT
{
    public class LocalizationKeyBuilder
    {
        readonly List<LocalizationKey> items = new List<LocalizationKey>();

        public LocalizationKeyBuilder Append(LocalizationKey key)
        {
            if (key.Empty)
            {
                return this;
            }

            if (items.Count == 0)
            {
                items.Add(key);
            }
            else if (key.Fixed)
            {
                var last = items[items.Count - 1];
                if (last.Fixed)
                {
                    last = last.Concat(key);
                    items[items.Count - 1] = last;
                }
                else
                {
                    items.Add(key);
                }
            }
            else
            {
                items.Add(key);
            }
            return this;
        }
        public LocalizationKeyBuilder AppendLine()
        {
            Append(LocalizationKey.CreateRaw(System.Environment.NewLine));
            return this;
        }
        public LocalizationKeyBuilder AppendLine(LocalizationKey key)
        {
            Append(key);
            AppendLine();
            return this;
        }

        public LocalizationKey ToLocalizationKey()
        {
            if (items.Count == 0)
            {
                return default;
            }
            if (items.Count == 1)
            {
                return items[0];
            }

            return items[0].Concat(items.Skip(1).ToArray());
        }

        public bool Empty
        {
            get
            {
                if (items.Count == 0)
                {
                    return true;
                }
                if (items.Count == 1)
                {
                    return items[0].Empty;
                }
                return false;
            }
        }

        public override string ToString()
        {
            return ToLocalizationKey().Localize();
        }

        public string ToString(SystemLanguage lang)
        {
            return ToLocalizationKey().Localize(lang);
        }
    }
}
