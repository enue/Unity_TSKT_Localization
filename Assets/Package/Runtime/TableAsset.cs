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
        public string[] sortedKeys = default;
        public Table.Language[] languages = default;

        public Table Table
        {
            get => new Table(sortedKeys, languages);
            set
            {
                sortedKeys = value.sortedKeys;
                languages = value.languages;
            }
        }

#if UNITY_EDITOR

        public string  GenerateCode()
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
                    builder.AppendLine("        public const int " + identifier + " = " + index.ToString() + ";");
                    ++index;
                }
                builder.AppendLine("    }");
            }

            builder.AppendLine();

            {
                builder.AppendLine("    public static class AutoLocalizationKey");
                builder.AppendLine("    {");

                var index = 0;
                foreach (var identifier in identifiers)
                {
                    builder.Append("        public static LocalizationKey " + identifier + " => new LocalizationKey(TableKey." + identifier + ");");

                    if (languages != null
                        && languages.Length > 0
                        && languages[0].words != null
                        && languages[0].words.Length > index
                        && languages[0].words[index] != null)
                    {
                        var word = languages[0].words[index]
                            .Replace("\n", null)
                            .Replace("\r", null);
                        builder.AppendLine(" // " + word);
                    }
                    else
                    {
                        builder.AppendLine();
                    }
                    ++index;
                }
                builder.AppendLine("    }");
            }

            builder.AppendLine("}");
            return builder.ToString();
        }
#endif
    }
}
