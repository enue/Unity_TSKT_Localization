#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

namespace TSKT.Localizations
{
    [CreateAssetMenu(fileName = "TableAsset", menuName = "TSKT/Localization Table", order = 1023)]
    public class TableAsset : ScriptableObject
    {
        public string[] sortedKeys = default!;
        public Table.Language[] languages = default!;

        public Table Table
        {
            get => new(sortedKeys, languages);
            set
            {
                sortedKeys = value.sortedKeys;
                languages = value.languages;
            }
        }

#if UNITY_EDITOR

        public string  GenerateCode(System.Text.RegularExpressions.Regex replacerRegex, bool smartReplace)
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendLine("namespace TSKT");
            builder.AppendLine("{");

            var identifiers = sortedKeys.Select(_ =>
                _.Replace('.', '_')
                .Replace("%", "percent")
                .Replace("+", "plus")
                .Replace("\'", "quotation")
                .Replace("?", "question")
                .Replace("？", "question"));

            {
                builder.AppendLine("    public static class TableKey");
                builder.AppendLine("    {");
                var index = 0;
                foreach (var identifier in identifiers)
                {
                    builder.AppendLine($"        public const int {identifier} = {index};");
                    ++index;
                }
                builder.AppendLine("    }");
            }

            builder.AppendLine();

            {
                builder.AppendLine("    public static class AutoLocalizationKey");
                builder.AppendLine("    {");

                foreach (var (identifier, index) in identifiers.Select((_, i) => (_, i)))
                {
                    string[]? replacers;
                    string? comment = null;

                    if (languages != null
                        && languages.Length > 0
                        && languages[0].words != null
                        && languages[0].words.Length > index
                        && languages[0].words[index] != null)
                    {
                        var word = languages[0].words[index];

                        replacers = replacerRegex.Matches(word)
                            .Select(_ => _.Value)
                            .ToArray();

                        word = word
                            .Replace("<", "&lt;")
                            .Replace(">", "&gt;")
                            .Replace("\r", null)
                            .Replace("\n", "<br/>");

                        // https://github.com/google/material-design-icons/issues/813#issuecomment-401601344
                        foreach (var it in word)
                        {
                            if (it >= 0xe000 && it <= 0xf000)
                            {
                                word = word.Replace(it.ToString(), $"\\u{((int)it):X}");
                            }
                        }
                        comment = word;
                    }
                    else
                    {
                        replacers = null;
                    }


                    if (!string.IsNullOrEmpty(comment))
                    {
                        builder.AppendLine("        /// <summary>");
                        builder.AppendLine($"        /// {comment}");
                        builder.AppendLine("        /// </summary>");
                    }

                    if (replacers == null || replacers.Length == 0)
                    {
                        builder.AppendLine($"        public static LocalizationKey {identifier} => new(TableKey.{identifier});");
                    }
                    else
                    {
                        var args = replacers
                            .Select(_ => _[1..^1])
                            .ToArray();
                        var argString = string.Join(", ", args.Select(_ => $"LocalizationKey {_}"));

                        builder.Append($"        public static LocalizationKey {identifier}({argString})");
                        builder.Append($" => new LocalizationKey(TableKey.{identifier})");

                        if (smartReplace)
                        {
                            foreach (var it in replacers.Zip(args, (replacer, arg) => (replacer, arg)))
                            {
                                builder.Append($".SmartReplace(\"{it.replacer}\", {it.arg})");
                            }
                            builder.AppendLine(";");
                        }
                        else if (replacers.Length == 1)
                        {
                            builder.AppendLine($".Replace(\"{replacers[0]}\", {args[0]});");
                        }
                        else
                        {
                            var replacerStrings = replacers.Zip(args, (replacer, arg) => (replacer, arg))
                                .Select(_ => $"(\"{_.replacer}\", {_.arg})");
                            var replacerString = string.Join(", ", replacerStrings);
                            builder.AppendLine($".Replace({replacerString});");
                        }

                        if (smartReplace)
                        {
                            if (!string.IsNullOrEmpty(comment))
                            {
                                builder.AppendLine("        /// <summary>");
                                builder.AppendLine($"        /// {comment}");
                                builder.AppendLine("        /// </summary>");
                            }
                            var _argString = string.Join(", ", args.Select(_ => $"string {_}"));

                            builder.Append($"        public static LocalizationKey {identifier}({_argString})");
                            builder.Append($" => new LocalizationKey(TableKey.{identifier})");
                            foreach (var it in replacers.Zip(args, (replacer, arg) => (replacer, arg)))
                            {
                                builder.Append($".SmartReplace(\"{it.replacer}\", {it.arg})");
                            }
                            builder.AppendLine(";");
                        }
                    }
                }
                builder.AppendLine("    }");
            }

            builder.AppendLine("}");
            return builder.ToString();
        }
#endif
    }
}
