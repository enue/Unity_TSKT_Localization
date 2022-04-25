#nullable enable
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
        readonly string? rawString;
        readonly string? key;
        readonly int? index;
        readonly ReadOnlyReactiveProperty<string>? reactive;

        public static explicit operator LocalizationKey(string rawString)
        {
            return CreateRaw(rawString);
        }
        static public LocalizationKey CreateRaw(string rawString)
        {
            return new LocalizationKey(
                rawString: rawString,
                rawKey: null,
                index: null,
                reactive: null);
        }

        LocalizationKey(string? rawString, string? rawKey, int? index, ReadOnlyReactiveProperty<string>? reactive)
        {
            this.rawString = rawString;
            key = rawKey;
            this.index = index;
            this.reactive = reactive;
        }

        public LocalizationKey(System.IObservable<string> reactive)
        {
            rawString = null;
            index = null;

            key = null;
            this.reactive = reactive.ToReadOnlyReactiveProperty();
        }
        public LocalizationKey(string key)
        {
            rawString = null;
            index = null;

            this.key = key;
            reactive = null;
        }

        public LocalizationKey(string key, params (string key, LocalizationKey value)[] args)
        {
            this = new LocalizationKey(key).Replace(args);
        }

        public LocalizationKey(int index)
        {
            rawString = null;
            key = null;

            this.index = index;
            reactive = null;
        }

        public LocalizationKey(int index, params (string key, LocalizationKey value)[] args)
        {
            this = new LocalizationKey(index).Replace(args);
        }

        readonly public LocalizationKey Replace(string key, string value)
        {
            if (Fixed)
            {
                var text = Localize().Replace(key, value);
                return CreateRaw(text);
            }

            return new LocalizationKey(ToReadOnlyReactiveProperty().Select(_ => _.Replace(key, value)));
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

            var origin = ToReadOnlyReactiveProperty()
                .Select(_ =>
                {
                    foreach (var it in args)
                    {
                        _ = _.Replace(it.key, it.value);
                    }
                    return _;
                });
            return new LocalizationKey(origin);
        }

        public readonly LocalizationKey Replace(string key, LocalizationKey value)
        {
            if (value.Fixed)
            {
                return Replace(key, value.Localize());
            }
            else if (Fixed)
            {
                var origin = Localize();
                if (value.Fixed)
                {
                    origin = origin.Replace(key, value.Localize());
                    return CreateRaw(origin);
                }

                var observable = value.ToReadOnlyReactiveProperty()
                    .Select(_ => origin.Replace(key, _));
                return new LocalizationKey(observable);
            }
            else
            {
                var observable = ToReadOnlyReactiveProperty().CombineLatest(value.ToReadOnlyReactiveProperty(), (_a, _b) =>
                {
                    return _a.Replace(key, _b);
                });
                return new LocalizationKey(observable);
            }
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

                var observable = Observable.CombineLatest(replacers.Select(_=>_.value.ToObservable()))
                    .Select(_ =>
                {
                    var result = origin;
                    foreach (var it in _.Select((_value, _index) => (_value, _index)))
                    {
                        result = result.Replace(replacers[it._index].key, it._value);
                    }
                    return result;
                });
                return new LocalizationKey(observable);
            }
            else
            {
                var replacers = args;
                var properties = new List<System.IObservable<string>>(replacers.Length + 1)
                {
                    ToReadOnlyReactiveProperty()
                };
                foreach (var it in replacers)
                {
                    properties.Add(it.value.ToObservable());
                }
                var observable = Observable.CombineLatest(properties).Select(_ =>
                {
                    var result = _[0];
                    foreach (var it in _.Skip(1).Select((_value, _index) => (_value, _index)))
                    {
                        result = result.Replace(replacers[it._index].key, it._value);
                    }
                    return result;
                });
                return new LocalizationKey(observable);
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
                var observable = right.ToReadOnlyReactiveProperty().Select(_ => left + _);
                return new LocalizationKey(observable);
            }
            else if (right.Fixed)
            {
                var _right = right.Localize();
                var observable = ToReadOnlyReactiveProperty().Select(_ => _ + _right);
                return new LocalizationKey(observable);
            }
            else
            {
                var observable = ToObservable().CombineLatest(right.ToObservable(), (_left, _right) => _left + _right);
                return new LocalizationKey(observable);
            }
        }
        public readonly LocalizationKey Concat(params LocalizationKey[] values)
        {
            var items = new List<LocalizationKey>();
            items.Add(this);

            foreach (var it in values)
            {
                if (it.Fixed)
                {
                    var last = items[^1];
                    if (last.Fixed)
                    {
                        items[^1] = last.Concat(it);
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

            var observable = Observable.CombineLatest(items.Select(_ => _.ToObservable()))
                .Select(_ =>
                {
                    var builder = new System.Text.StringBuilder();
                    foreach (var it in _)
                    {
                        builder.Append(_);
                    }
                    return builder.ToString();
                });
            return new LocalizationKey(observable);
        }

        readonly public LocalizationKey Trim()
        {
            if (Fixed)
            {
                var result = Localize().Trim();
                return CreateRaw(result);
            }
            else
            {
                var observable = ToObservable().Select(_ => _.Trim());
                return new LocalizationKey(observable);
            }
        }

        readonly public string Localize()
        {
            return Localize(Localization.CurrentLanguage);
        }

        public readonly string Localize(SystemLanguage language)
        {
            if (reactive != null)
            {
                return reactive.Value;
            }
            else if (rawString != null)
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
                    rawString == null
                    && key == null
                    && !index.HasValue
                    && reactive == null;
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
                if (reactive != null)
                {
                    return false;
                }
                return rawString != null;
            }
        }

        public System.IObservable<string> ToObservable()
        {
            return ToReadOnlyReactiveProperty().AsObservable();
        }
        public ReadOnlyReactiveProperty<string> ToReadOnlyReactiveProperty()
        {
            if (Fixed)
            {
                return new ReactiveProperty<string>(Localize()).ToReadOnlyReactiveProperty();
            }
            if (reactive != null)
            {
                return reactive;
            }

            var clone = this;
            return Localization.currentLanguage
                .Select(_ => clone.Localize(_))
                .ToReadOnlyReactiveProperty();
        }

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
