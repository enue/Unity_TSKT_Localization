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

            builder.AppendLine("    public static class TableKey");
            builder.AppendLine("    {");
            for (int index = 0; index < sortedKeys.Length; ++index)
            {
                var identifier = sortedKeys[index]
                    .Replace('.', '_')
                    .Replace("%", "percent")
                    .Replace("+", "plus")
                    .Replace("\'", "quotation")
                    .Replace("?", "question")
                    .Replace("？", "question");
                builder.AppendLine("        public const int " +  identifier + " = " + index.ToString() + ";");
            }
            builder.AppendLine("    }");
            builder.AppendLine("}");
            return builder.ToString();
        }
#endif
    }
}
