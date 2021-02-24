using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
        readonly System.Func<SystemLanguage, string> factory;

        static public LocalizationKey CreateRaw(string rawString)
        {
            return new LocalizationKey(
                hasRawString: true,
                rawString: rawString,
                rawKey: null,
                index: null,
                func: null);
        }

        LocalizationKey(bool hasRawString, string rawString, string rawKey, int? index, System.Func<SystemLanguage, string> func)
        {
            this.hasRawString = hasRawString;
            this.rawString = rawString;
            key = rawKey;
            this.index = index;
            factory = func;
        }

        LocalizationKey(System.Func<SystemLanguage, string> factory)
        {
            hasRawString = false;
            rawString = null;
            index = null;

            key = null;
            this.factory = factory;
        }

        public LocalizationKey(string key)
        {
            hasRawString = false;
            rawString = null;
            index = null;

            this.key = key;
            factory = null;
        }

        public LocalizationKey(string key, params (string key, LocalizationKey value)[] args)
        {
            this = new LocalizationKey(key).Replace(args);
        }

        public LocalizationKey(int index)
        {
            hasRawString = false;
            rawString = null;
            key = null;

            this.index = index;
            factory = null;
        }

        public LocalizationKey(int index, params (string key, LocalizationKey value)[] args)
        {
            this = new LocalizationKey(index).Replace(args);
        }

        readonly public LocalizationKey Replace(params (string key, string value)[] args)
        {
            if (Fixed)
            {
                var text = Localize();
                foreach(var it in args)
                {
                    text = text.Replace(it.key, it.value);
                }
                return CreateRaw(text);
            }

            var origin = this;
            return new LocalizationKey(factory: _ =>
            {
                var result = origin.Localize(_);
                foreach (var it in args)
                {
                    result = result.Replace(it.key, it.value);
                }
                return result;
            });
        }

        readonly public LocalizationKey Replace(params (string key, LocalizationKey value)[] args)
        {
            if (Fixed)
            {
                var origin = Localize();
                var fixedCount = 0;
                for (int i = 0; i < args.Length; ++i)
                {
                    if (args[i].value.Fixed)
                    {
                        origin = origin.Replace(args[i].key, args[i].value.Localize());
                    }
                    else
                    {
                        break;
                    }
                    ++fixedCount;
                }
                (string key, LocalizationKey value)[] replacers;

                if (fixedCount == args.Length)
                {
                    return CreateRaw(origin);
                }
                else if (fixedCount == 0)
                {
                    replacers = args;
                }
                else
                {
                    replacers = args.Skip(fixedCount).ToArray();
                }

                return new LocalizationKey(factory: _ =>
                {
                    var result = origin;
                    foreach (var it in replacers)
                    {
                        result = result.Replace(it.key, it.value.Localize(_));
                    }
                    return result;
                });
            }
            else
            {
                var origin = this;
                return new LocalizationKey(factory: _ =>
                {
                    var result = origin.Localize(_);
                    foreach (var it in args)
                    {
                        result = result.Replace(it.key, it.value.Localize(_));
                    }
                    return result;
                });
            }
        }
        readonly public LocalizationKey Concat(LocalizationKey right)
        {
            if (Fixed && right.Fixed)
            {
                return CreateRaw(Localize() + right.Localize());
            }
            else if (Fixed)
            {
                var left = Localize();
                return new LocalizationKey(factory: _ =>
                {
                    return left + right.Localize(_);
                });
            }
            else if (right.Fixed)
            {
                var left = this;
                var _right = right.Localize();
                return new LocalizationKey(factory: _ =>
                {
                    return left.Localize(_) + _right;
                });
            }
            else
            {
                var left = this;
                return new LocalizationKey(factory: _ =>
                {
                    return left.Localize(_) + right.Localize(_);
                });
            }
        }
        readonly public LocalizationKey Concat(params LocalizationKey[] values)
        {
            var items = new List<LocalizationKey>();
            items.Add(this);

            foreach (var it in values)
            {
                if (it.Fixed)
                {
                    var last = items[items.Count - 1];
                    if (last.Fixed)
                    {
                        items[items.Count - 1] = last.Concat(it);
                    }
                    else
                    {
                        items.Add(it);
                    }
                }
                else
                {
                    items.Add(it);
                }
            }

            if (items.Count == 1)
            {
                return items[0];
            }

            return new LocalizationKey(factory: lang =>
            {
                var builder = new System.Text.StringBuilder();
                foreach (var it in items)
                {
                    builder.Append(it.Localize(lang));
                }
                return builder.ToString();
            });

        }

        readonly public string Localize()
        {
            return Localize(Localization.CurrentLanguage);
        }

        readonly public string Localize(SystemLanguage language)
        {
            if (factory != null)
            {
                return factory(language);
            }
            else if (hasRawString)
            {
                return rawString;
            }
            else if (key != null)
            {
                return Localization.Get(language, key);
            }
            else if (index.HasValue)
            {
                return Localization.Get(language, index.Value);
            }
            else
            {
                // empty
                return "";
            }
        }

        readonly public bool Empty
        {
            get
            {
                return
                    !hasRawString
                    && key == null
                    && !index.HasValue
                    && factory == null;
            }
        }

        public bool Fixed
        {
            get
            {
                if (Empty)
                {
                    return true;
                }
                if (factory != null)
                {
                    return false;
                }
                return hasRawString;
            }
        }

#if TSKT_LOCALIZATION_SUPPORT_UNIRX
        public ReadOnlyReactiveProperty<string> ToReadOnlyReactiveProperty()
        {
            if (Fixed)
            {
                return new ReactiveProperty<string>(Localize()).ToReadOnlyReactiveProperty();
            }

            var clone = this;
            return Localization.currentLanguage
                .Select(_ => clone.Localize(_))
                .ToReadOnlyReactiveProperty();
        }
#endif

        static public LocalizationKey Join(in LocalizationKey separator, IEnumerable<LocalizationKey> values)
        {
            var builder = new LocalizationKeyBuilder();
            var index = 0;
            foreach (var it in values)
            {
                if (index > 0)
                {
                    builder.Append(separator);
                }
                builder.Append(it);
                ++index;
            }
            return builder.ToLocalizationKey();
        }
        static public LocalizationKey Join(in LocalizationKey separator, params LocalizationKey[] values)
        {
            var builder = new LocalizationKeyBuilder();
            var index = 0;
            foreach (var it in values)
            {
                if (index > 0)
                {
                    builder.Append(separator);
                }
                builder.Append(it);
                ++index;
            }
            return builder.ToLocalizationKey();
        }
    }
}
