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
                    obj.Table = Sheet.CreateFromFolder(folder).ToTable();
                    EditorUtility.SetDirty(obj);
                }
            }

            if (GUILayout.Button("Generate Code"))
            {
                var obj = (TableAsset)target;
                obj.GenerateCode();
            }

            if (GUILayout.Button("Stat"))
            {
                var table = ((TableAsset)target).Table;
                var wordCountMap = table.WordCountMap;

                foreach (var it in table.languages)
                {
                    Debug.Log(it.code
                        + ", length : " + table.GetTotalLength(it.code)
                        + ", word : " + wordCountMap[it.code]);
                }
            }

            if (GUILayout.Button("Verify"))
            {
                var obj = (TableAsset)target;
                obj.Table.Verify(SystemLanguage.Japanese);
            }

            DrawDefaultInspector();
        }
    }
}
