using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace TSKT.Localizations
{
    [CustomEditor(typeof(TableAsset))]
    public class TableAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Read From Files"))
            {
                var folder = EditorUtility.OpenFolderPanel("select localization json folder", "", "");
                if (!string.IsNullOrEmpty(folder))
                {
                    var obj = (TableAsset)target;

                    var setting = LocalizationSetting.Instance;
                    Debug.Assert(setting, "not found LocalizationSetting");

                    if (setting)
                    {
                        obj.Table = Sheet.CreateFromFolder(folder).ToTable(setting.Languages);
                        EditorUtility.SetDirty(obj);
                    }
                }
            }

            if (GUILayout.Button("Generate key CS"))
            {
                var filename = EditorUtility.SaveFilePanelInProject("generate csharp code", "TableKey", "cs", "");
                if (!string.IsNullOrEmpty(filename))
                {
                    var obj = (TableAsset)target;
                    var code = obj.GenerateCode(LocalizationSetting.Instance.ReplacerRegex, LocalizationSetting.Instance.SmartReplace);

                    File.WriteAllText(filename, code);
                }
            }

            if (GUILayout.Button("Stat"))
            {
                var table = ((TableAsset)target).Table;
                var keyCount = table.sortedKeys.Length;
                Debug.Log("key count : " + keyCount);

                var wordCountMap = table.WordCountMap;
                foreach (var it in table.languages)
                {
                    Debug.Log(it.code
                        + ", word : " + wordCountMap[it.code]
                        + ", length : " + table.GetTotalLength(it.code));
                }
            }

            DrawDefaultInspector();
        }
    }
}
