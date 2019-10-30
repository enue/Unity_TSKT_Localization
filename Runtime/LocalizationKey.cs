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
        readonly (string key, string value)[] args;

        static public LocalizationKey CreateRaw(string rawString)
        {
            return new LocalizationKey(
                hasRawString: true,
                rawString: rawString,
                rawKey: null,
                index: null,
                args: null);
        }

        LocalizationKey(bool hasRawString, string rawString, string rawKey, int? index, (string key, string value)[] args)
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

        public LocalizationKey(string key, params (string key, string value)[] args)
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

        public LocalizationKey(int index, params (string key, string value)[] args)
        {
            hasRawString = false;
            rawString = null;
            rawKey = null;

            this.index = index;
            this.args = args;
        }

        public string Localize()
        {
            if (hasRawString)
            {
                return rawString;
            }
            if (rawKey != null)
            {
                if (args != null && args.Length > 0)
                {
                    return Localization.Get(rawKey, args);
                }
                else
                {
                    return Localization.Get(rawKey);
                }
            }

            if (index.HasValue)
            {
                if (args != null && args.Length > 0)
                {
                    return Localization.Get(index.Value, args);
                }
                else
                {
                    return Localization.Get(index.Value);
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
