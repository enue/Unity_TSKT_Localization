using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if TSKT_LOCALIZATION_SUPPORT_UNIRX
using UniRx;

namespace TSKT
{
    public readonly struct LocalizationKey
    {
        public readonly ReactiveProperty<SystemLanguage> language;
        public readonly ReadOnlyReactiveProperty<string> property;

        static public LocalizationKey CreateRaw(string rawString)
        {
            var language = new ReactiveProperty<SystemLanguage>(Localization.currentLanguage.Value);
            var property = new ReactiveProperty<string>(rawString).ToReadOnlyReactiveProperty();
            return new LocalizationKey(language, property);
        }

        LocalizationKey(ReactiveProperty<SystemLanguage> language, ReadOnlyReactiveProperty<string> property)
        {
            this.language = language;
            this.property = property;
        }

        public LocalizationKey(string key)
        {
            language = new ReactiveProperty<SystemLanguage>(Localization.currentLanguage.Value);
            property = language
                .Select(_ => Localization.Get(_, key))
                .ToReadOnlyReactiveProperty();
        }

        public LocalizationKey(string key, params (string key, string value)[] args)
        {
            language = new ReactiveProperty<SystemLanguage>(Localization.currentLanguage.Value);
            property = language
                .Select(lang =>
                {
                    var t = Localization.Get(lang, key);
                    foreach (var it in args)
                    {
                        t = t.Replace(it.key, it.value);
                    }
                    return t;
                })
                .ToReadOnlyReactiveProperty();
        }

        public LocalizationKey(string key, params (string key, LocalizationKey value)[] args)
        {
            language = new ReactiveProperty<SystemLanguage>(Localization.currentLanguage.Value);
            property = language
                .Select(lang =>
                {
                    var t = Localization.Get(lang, key);
                    foreach (var it in args)
                    {
                        t = t.Replace(it.key, it.value.Localize(lang));
                    }
                    return t;
                })
                .ToReadOnlyReactiveProperty();
        }

        public LocalizationKey(int index)
        {
            language = new ReactiveProperty<SystemLanguage>(Localization.currentLanguage.Value);
            property = language
                .Select(_ => Localization.Get(_, index))
                .ToReadOnlyReactiveProperty();
        }

        public LocalizationKey(int index, params (string key, LocalizationKey value)[] args)
        {
            language = new ReactiveProperty<SystemLanguage>(Localization.currentLanguage.Value);
            property = language
                .Select(lang =>
                {
                    var t = Localization.Get(lang, index);
                    foreach (var it in args)
                    {
                        t = t.Replace(it.key, it.value.Localize(lang));
                    }
                    return t;
                })
                .ToReadOnlyReactiveProperty();
        }

        public LocalizationKey(int index, params (string key, string value)[] args)
        {
            language = new ReactiveProperty<SystemLanguage>(Localization.currentLanguage.Value);
            property = language
                .Select(lang =>
                {
                    var t = Localization.Get(lang, index);
                    foreach (var it in args)
                    {
                        t = t.Replace(it.key, it.value);
                    }
                    return t;
                })
                .ToReadOnlyReactiveProperty();
        }


        readonly public LocalizationKey Replace(params (string key, string value)[] args)
        {
            var prop = property?.Select(_ =>
            {
                foreach (var (key, value) in args)
                {
                    _ = _.Replace(key, value);
                }
                return _;
            }).ToReadOnlyReactiveProperty();

            return new LocalizationKey(language, prop);
        }

        readonly public LocalizationKey Replace(params (string key, LocalizationKey value)[] args)
        {
            var lang = language;
            var prop = property?.Select(_ =>
            {
                foreach (var (key, value) in args)
                {
                    _ = _.Replace(key, value.Localize(lang.Value));
                }
                return _;
            }).ToReadOnlyReactiveProperty();

            return new LocalizationKey(language, prop);
        }

        readonly public string Localize()
        {
            if (language != null)
            {
                language.Value = Localization.CurrentLanguage;
            }
            return property?.Value ?? "";
        }

        readonly public string Localize(SystemLanguage language)
        {
            if (this.language != null)
            {
                this.language.Value = language;
            }
            return property?.Value ?? "";
        }

        readonly public bool Empty
        {
            get
            {
                if (property == null)
                {
                    return true;
                }
                return false;
            }
        }
    }
}

#else

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

        readonly public LocalizationKey Replace(params (string key, string value)[] args)
        {
            if (this.args == null || this.args.Length == 0)
            {
                return new LocalizationKey(hasRawString, rawString, rawKey, index, System.Array.Empty<(string key, LocalizationKey value)>());
            }

            var builder = new ArrayBuilder<(string, LocalizationKey)>(this.args.Length + args.Length);
            foreach (var it in this.args)
            {
                builder.Add(it);
            }
            foreach (var (key, value) in args)
            {
                builder.Add((key, CreateRaw(value)));
            }
            return new LocalizationKey(hasRawString, rawString, rawKey, index, builder.Array);
        }

        readonly public LocalizationKey Replace(params (string key, LocalizationKey value)[] args)
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

        readonly public string Localize()
        {
            if (hasRawString)
            {
                return rawString;
            }
            return Localize(Localization.CurrentLanguage);
        }

        readonly public string Localize(SystemLanguage language)
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

        readonly public bool Empty
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
#endif
