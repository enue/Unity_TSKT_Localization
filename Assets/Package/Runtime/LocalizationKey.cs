#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using R3;
using System;
using Cysharp.Text;

namespace TSKT
{
    public readonly struct LocalizationKey
    {
        readonly string? fixedValue;
        readonly ReadOnlyReactiveProperty<string>? reactive;

        public static explicit operator LocalizationKey(string rawString)
        {
            return CreateRaw(rawString);
        }
        static public LocalizationKey CreateRaw(string rawString)
        {
            return new LocalizationKey(rawString, reactive: null);
        }

        LocalizationKey(string? fixeValue, ReadOnlyReactiveProperty<string>? reactive)
        {
            fixedValue = fixeValue;
            this.reactive = reactive;
        }

        public LocalizationKey(ReadOnlyReactiveProperty<string> reactive)
        {
            fixedValue = null;
            this.reactive = reactive;
        }
        public LocalizationKey(string key)
        {
            if (Localization.Languages?.Length == 1)
            {
                fixedValue = Localization.Get(key);
                reactive = null;
            }
            else
            {
                fixedValue = null;
                reactive = Localization.currentLanguage.Select(key, static (_, _key) => Localization.Get(_key)).ToReadOnlyReactiveProperty("");
            }
        }

        public LocalizationKey(int index)
        {
            if (Localization.Languages?.Length == 1)
            {
                fixedValue = Localization.Get(index);
                reactive = null;
            }
            else
            {
                fixedValue = null;
                reactive = Localization.currentLanguage.Select(index, static (_, _index) => Localization.Get(_index)).ToReadOnlyReactiveProperty("");
            }
        }

        public readonly LocalizationKey SmartReplace(string key, string value)
        {
            return SmartReplace(key, CreateRaw(value));
        }

        public readonly LocalizationKey SmartReplace(string key, LocalizationKey value)
        {
            if (Fixed && value.Fixed)
            {
                var text = DoReplace(Localize(), key, value.Localize());
                return CreateRaw(text);
            }
            if (Fixed)
            {
                var observable = value.reactive!.Select((origin: Localize(), key), static (_, _states) => DoReplace(_states.origin, _states.key, _)).ToReadOnlyReactiveProperty("");
                return new LocalizationKey(observable);
            }
            if (value.Fixed)
            {
                var observable = reactive!.Select((key, value: value.Localize()), static (_, _states) => DoReplace(_, _states.key, _states.value)).ToReadOnlyReactiveProperty("");
                return new LocalizationKey(observable);
            }
            {
                var observable = reactive!
                    .CombineLatest(value.reactive!, (_origin, _value) => DoReplace(_origin, key, _value))
                    .ToReadOnlyReactiveProperty("");
                return new LocalizationKey(observable);
            }

            static string DoReplace(string origin, string key, string _value)
            {
                // {{key}:ordinal}
                {
                    var k = $"{{{key}:ordinal}}";
                    if (origin.Contains(k))
                    {
                        return origin.Replace(k, Localization.ToOrdinal(_value));
                    }
                }
                // {{key}:plural:an apple|_ apples}
                {
                    var word = $"{{{key}:plural:";
                    var index = origin.IndexOf(word);
                    if (index != -1)
                    {
                        var first = index + word.Length;
                        var second = origin.IndexOf('|', first) + 1;
                        if (second != 0)
                        {
                            var closing = origin.IndexOf('}', second + 1);
                            if (closing != -1)
                            {
                                using var newValue = ZString.CreateStringBuilder();
                                if (_value == "1")
                                {
                                    newValue.Append(origin.AsSpan(first, second - first - 1));
                                }
                                else
                                {
                                    newValue.Append(origin.AsSpan(second, closing - second));
                                }
                                newValue.Replace("_", _value);

                                using var originBuilder = ZString.CreateStringBuilder();
                                originBuilder.Append(origin);
                                originBuilder.Remove(index, closing - index + 1);
                                originBuilder.Insert(index, newValue.AsSpan(), 1);
                                return originBuilder.ToString();
                            }
                        }
                    }
                }
                return origin.Replace(key, _value);

            }
        }

        readonly public LocalizationKey Replace(string key, string value)
        {
            if (Fixed)
            {
                var text = Localize().Replace(key, value);
                return CreateRaw(text);
            }

            return new LocalizationKey(reactive!.Select((key, value), static (_, states) => _.Replace(states.key, states.value)).ToReadOnlyReactiveProperty(""));
        }

        public readonly LocalizationKey Replace(params (string key, string value)[] args)
        {
            if (Fixed)
            {
                using var builder = ZString.CreateStringBuilder();
                builder.Append(Localize());
                foreach (var (key, value) in args)
                {
                    builder.Replace(key, value);
                }
                return CreateRaw(builder.ToString());
            }

            var observable = reactive!
                .Select(args, static (_, _args) =>
                {
                    using var builder = ZString.CreateStringBuilder();
                    builder.Append(_);
                    foreach (var (key, value) in _args)
                    {
                        builder.Replace(key, value);
                    }
                    return _;
                })
                .ToReadOnlyReactiveProperty("");
            return new LocalizationKey(observable);
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
                var observable = value.reactive!
                    .Select((key, origin), static (_, _states) => _states.origin.Replace(_states.key, _))
                    .ToReadOnlyReactiveProperty("");
                return new LocalizationKey(observable);
            }
            else
            {
                var observable = reactive!.CombineLatest(value.reactive!, (_a, _b) =>
                {
                    return _a.Replace(key, _b);
                }).ToReadOnlyReactiveProperty("");
                return new LocalizationKey(observable);
            }
        }

        public readonly LocalizationKey Replace(params (string key, LocalizationKey value)[] args)
        {
            if (Fixed)
            {
                using var origin = ZString.CreateStringBuilder();
                origin.Append(Localize());
                var fixedCount = 0;
                for (int i = 0; i < args.Length; ++i)
                {
                    if (args[i].value.Fixed)
                    {
                        origin.Replace(args[i].key, args[i].value.Localize());
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
                    return CreateRaw(origin.ToString());
                }
                else if (fixedCount == 0)
                {
                    replacers = args;
                }
                else
                {
                    replacers = args.Skip(fixedCount).ToArray();
                }

                var observable = Observable.CombineLatest(replacers.Select(static _ => _.value.ToReadOnlyReactiveProperty()))
                    .Select((origin: origin.ToString(), replacers), static (_, _states) =>
                {
                    using var result = ZString.CreateStringBuilder();
                    result.Append(_states.origin);
                    var index = 0;
                    foreach (var it in _)
                    {
                        result.Replace(_states.replacers[index].key, it);
                        ++index;
                    }
                    return result.ToString();
                }).ToReadOnlyReactiveProperty("");
                return new LocalizationKey(observable);
            }
            else
            {
                var replacers = args;
                var properties = new List<Observable<string>>(replacers.Length + 1)
                {
                    ToReadOnlyReactiveProperty()
                };
                foreach (var (key, value) in replacers)
                {
                    properties.Add(value.ToReadOnlyReactiveProperty());
                }
                var observable = Observable.CombineLatest(properties).Select(replacers, static (_, _replacers) =>
                {
                    using var result = ZString.CreateStringBuilder();
                    result.Append(_[0]);
                    foreach (var it in _.Skip(1).Select(static (_value, _index) => (_value, _index)))
                    {
                        result.Replace(_replacers[it._index].key, it._value);
                    }
                    return result.ToString();
                }).ToReadOnlyReactiveProperty("");
                return new LocalizationKey(observable);
            }
        }
        public readonly LocalizationKey Replace(char oldChar, char newChar)
        {
            if (Fixed)
            {
                var text = Localize().Replace(oldChar, newChar);
                return CreateRaw(text);
            }

            return new LocalizationKey(reactive!.Select((oldChar, newChar), static (_, _states) => _.Replace(_states.oldChar, _states.newChar)).ToReadOnlyReactiveProperty(""));
        }
        readonly public LocalizationKey Concat(LocalizationKey right)
        {
            if (Fixed && right.Fixed)
            {
                return CreateRaw(ZString.Concat(Localize(), right.Localize()));
            }
            else if (Fixed)
            {
                var left = Localize();
                var observable = right.reactive!.Select(left, static (_, _left) => ZString.Concat(_left, _)).ToReadOnlyReactiveProperty("");
                return new LocalizationKey(observable);
            }
            else if (right.Fixed)
            {
                var _right = right.Localize();
                var observable = reactive!.Select(_right, static (_, right) => ZString.Concat(_, right)).ToReadOnlyReactiveProperty("");
                return new LocalizationKey(observable);
            }
            else
            {
                var observable = reactive!.CombineLatest(right.reactive!, (_left, _right) => ZString.Concat(_left, _right)).ToReadOnlyReactiveProperty("");
                return new LocalizationKey(observable);
            }
        }
        public readonly LocalizationKey Concat(params LocalizationKey[] values)
        {
            var items = new List<LocalizationKey>
            {
                this
            };

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

            var observable = Observable.CombineLatest(items.Select(static _ => _.ToReadOnlyReactiveProperty()))
                .Select(static _ =>
                {
                    using var builder = ZString.CreateStringBuilder();
                    foreach (var it in _)
                    {
                        builder.Append(it);
                    }
                    return builder.ToString();
                }).ToReadOnlyReactiveProperty("");
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
                var observable = reactive!.Select(_ => _.Trim()).ToReadOnlyReactiveProperty("");
                return new LocalizationKey(observable);
            }
        }

        public readonly string Localize()
        {
            return reactive?.CurrentValue ?? fixedValue ?? "";
        }

        public readonly bool Empty
        {
            get
            {
                return reactive == null && string.IsNullOrEmpty(fixedValue);
            }
        }

        public bool Fixed => reactive == null;

        public ReadOnlyReactiveProperty<string> ToReadOnlyReactiveProperty()
        {
            if (reactive != null)
            {
                return reactive;
            }
            var rp = new ReactiveProperty<string>(Localize());
            rp.OnCompleted();
            return rp;
        }

#if TSKT_UI_SUPPORT
        public void SubscribeToText(UnityEngine.UI.Text text, int throttleFrame = 0)
        {
            if (Fixed)
            {
                text.text = Localize();
                return;
            }
            if (Localization.Languages?.Length == 1)
            {
                text.text = Localize();
                return;
            }
            if (throttleFrame == 0)
            {
                TextSubscriber.Subscribe(text, ToReadOnlyReactiveProperty());
            }
            else
            {
                TextSubscriber.Subscribe(text, ToReadOnlyReactiveProperty().ThrottleLastFrame(throttleFrame));
            }
        }

        public void SubscribeToText(TMPro.TMP_Text text)
        {
            if (Fixed)
            {
                text.text = Localize();
                return;
            }
            if (Localization.Languages?.Length == 1)
            {
                text.text = Localize();
                return;
            }
            TextSubscriber.Subscribe(text, ToReadOnlyReactiveProperty());
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
