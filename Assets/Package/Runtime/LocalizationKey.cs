using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TSKT_LOCALIZATION_SUPPORT_UNIRX
using UniRx;
#endif

namespace TSKT
{
    public readonly struct LocalizationKey
    {
        readonly bool hasRawString;
        readonly string rawString;
        readonly string key;
        readonly int? index;
        readonly (string key, LocalizationKey value)[] replacers;

        static public LocalizationKey CreateRaw(string rawString)
        {
            return new LocalizationKey(
                hasRawString: true,
                rawString: rawString,
                rawKey: null,
                index: null,
                args: null);
        }

        LocalizationKey(bool hasRawString, string rawString, string rawKey, int? index, (string key, LocalizationKey value)[] args)
        {
            this.hasRawString = hasRawString;
            this.rawString = rawString;
            key = rawKey;
            this.index = index;
            replacers = args;
        }

        public LocalizationKey(string key)
        {
            hasRawString = false;
            rawString = null;
            index = null;

            this.key = key;
            replacers = null;
        }

        public LocalizationKey(string key, params (string key, LocalizationKey value)[] args)
        {
            hasRawString = false;
            rawString = null;
            index = null;

            this.key = key;
            replacers = args;
        }

        public LocalizationKey(int index)
        {
            hasRawString = false;
            rawString = null;
            key = null;

            this.index = index;
            replacers = null;
        }

        public LocalizationKey(int index, params (string key, LocalizationKey value)[] args)
        {
            hasRawString = false;
            rawString = null;
            key = null;

            this.index = index;
            replacers = args;
        }

        readonly public LocalizationKey Replace(params (string key, string value)[] args)
        {
            var builder = new ArrayBuilder<(string, LocalizationKey)>((replacers?.Length ?? 0) + args.Length);
            if (replacers != null)
            {
                foreach (var it in replacers)
                {
                    builder.Add(it);
                }
            }
            foreach (var (key, value) in args)
            {
                builder.Add((key, CreateRaw(value)));
            }
            return new LocalizationKey(hasRawString, rawString, key, index, builder.Array);
        }

        readonly public LocalizationKey Replace(params (string key, LocalizationKey value)[] args)
        {
            if (replacers == null || replacers.Length == 0)
            {
                return new LocalizationKey(hasRawString, rawString, key, index, args);
            }

            var builder = new ArrayBuilder<(string, LocalizationKey)>(replacers.Length + args.Length);
            foreach (var it in replacers)
            {
                builder.Add(it);
            }
            foreach(var it in args)
            {
                builder.Add(it);
            }
            return new LocalizationKey(hasRawString, rawString, key, index, builder.Array);
        }

        readonly public string Localize()
        {
            return Localize(Localization.CurrentLanguage);
        }

        readonly public string Localize(SystemLanguage language)
        {
            string result;
            if (hasRawString)
            {
                result = rawString;
            }
            else if (key != null)
            {
                result = Localization.Get(language, key);
            }
            else if (index.HasValue)
            {
                result = Localization.Get(language, index.Value);
            }
            else
            {
                // empty
                return "";
            }

            if (replacers != null)
            {
                foreach (var it in replacers)
                {
                    result = result.Replace(it.key, it.value.Localize(language));
                }
            }

            return result;
        }

        readonly public bool Empty
        {
            get
            {
                return
                    !hasRawString
                    && key == null
                    && !index.HasValue;
            }
        }

#if TSKT_LOCALIZATION_SUPPORT_UNIRX
        public ReadOnlyReactiveProperty<string> ToReadOnlyReactiveProperty()
        {
            var clone = this;
            return Localization.currentLanguage
                .Select(_ => clone.Localize(_))
                .ToReadOnlyReactiveProperty();
        }
#endif
    }
}
