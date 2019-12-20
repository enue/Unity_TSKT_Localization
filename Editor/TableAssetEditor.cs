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
                var obj = (TableAsset)target;
                obj.Table = Sheet.CreateFromFiles().ToTable();
                EditorUtility.SetDirty(obj);
            }

            if (GUILayout.Button("Generate Code"))
            {
                var obj = (TableAsset)target;
                obj.GenerateCode();
            }

            if (GUILayout.Button("Stat"))
            {
                var obj = (TableAsset)target;
                Debug.Log("japanese length : " + obj.Table.JapaneseTotalLength.ToString());

                foreach(var it in LocalizationSetting.Instance.Languages)
                {
                    if (it != SystemLanguage.Japanese)
                    {
                        var wordCountMap = obj.Table.WordCountMap;
                        var coverage = (float)wordCountMap[it] / wordCountMap[SystemLanguage.Japanese];
                        Debug.Log(it.ToString() + " coverage : " + coverage.ToString());
                    }
                }
            }

            if (GUILayout.Button("Verify"))
            {
                var obj = (TableAsset)target;
                obj.Table.Verify();
            }

            DrawDefaultInspector();
        }
    }
}
