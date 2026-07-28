#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TSKT
{
    public class LocalizationKeyBuilder
    {
        readonly List<LocalizationKey> items = new();
        public LocalizationKeyBuilder Prepend(LocalizationKey key)
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
                var first = items[0];
                if (first.Fixed)
                {
                    first = first.Concat(key);
                    items[0] = first;
                }
                else
                {
                    items.Insert(0, key);
                }
            }
            else
            {
                items.Insert(0, key);
            }
            return this;
        }

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
                var last = items[^1];
                if (last.Fixed)
                {
                    last = last.Concat(key);
                    items[^1] = last;
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
    }
}
