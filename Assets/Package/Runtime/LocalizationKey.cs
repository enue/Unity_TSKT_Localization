using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSKT
{
    public readonly struct LocalizationKey
    {
        readonly bool hasRawString;
        readonly string rawString;
        readonly string rawKey;
        readonly int? index;
        readonly (string key, LocalizationKey value)[] args;

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
            this.rawKey = rawKey;
            this.index = index;
            this.args = args;
        }

        public LocalizationKey(string key)
        {
            hasRawString = false;
            rawString = null;
            index = null;

            rawKey = key;
            args = null;
        }

        public LocalizationKey(string key, params (string key, LocalizationKey value)[] args)
        {
            hasRawString = false;
            rawString = null;
            index = null;

            rawKey = key;
            this.args = args;
        }

        public LocalizationKey(int index)
        {
            hasRawString = false;
            rawString = null;
            rawKey = null;

            this.index = index;
            args = null;
        }

        public LocalizationKey(int index, params (string key, LocalizationKey value)[] args)
        {
            hasRawString = false;
            rawString = null;
            rawKey = null;

            this.index = index;
            this.args = args;
        }

        public LocalizationKey Replace(params (string key, LocalizationKey value)[] args)
        {
            if (this.args == null || this.args.Length == 0)
            {
                return new LocalizationKey(hasRawString, rawString, rawKey, index, args);
            }

            var builder = new ArrayBuilder<(string, LocalizationKey)>(this.args.Length + args.Length);
            foreach (var it in this.args)
            {
                builder.Add(it);
            }
            foreach(var it in args)
            {
                builder.Add(it);
            }
            return new LocalizationKey(hasRawString, rawString, rawKey, index, builder.Array);
        }

        public string Localize()
        {
            if (hasRawString)
            {
                return rawString;
            }
            return Localize(Localization.CurrentLanguage);
        }

        public string Localize(SystemLanguage language)
        {
            if (hasRawString)
            {
                return rawString;
            }
            if (rawKey != null)
            {
                if (args != null && args.Length > 0)
                {
                    return Localization.Get(language, rawKey, args);
                }
                else
                {
                    return Localization.Get(language, rawKey);
                }
            }

            if (index.HasValue)
            {
                if (args != null && args.Length > 0)
                {
                    return Localization.Get(language, index.Value, args);
                }
                else
                {
                    return Localization.Get(language, index.Value);
                }
            }

            // empty
            return "";
        }

        public bool Empty
        {
            get
            {
                return
                    !hasRawString
                    && rawKey == null
                    && !index.HasValue;
            }
        }
    }
}
