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
            if (GUILayout.Button("Build"))
            {
                var obj = (TableAsset)target;
                obj.Build();
                EditorUtility.SetDirty(obj);

                obj.GenerateCode();
            }

            if (GUILayout.Button("Stat"))
            {
                var obj = (TableAsset)target;
                Debug.Log("japanese length : " + obj.JapaneseTotalLength.ToString());

                var wordCountMap = obj.WordCountMap;
                var englishCoverage = (float)wordCountMap[SystemLanguage.English] / wordCountMap[SystemLanguage.Japanese];
                Debug.Log("Enghlish coverage : " + englishCoverage.ToString());

                if (wordCountMap.ContainsKey(SystemLanguage.ChineseSimplified))
                {
                    var simplifiedChineseCoverage = (float)wordCountMap[SystemLanguage.ChineseSimplified] / wordCountMap[SystemLanguage.Japanese];
                    Debug.Log("Simplified chinese coverage : " + simplifiedChineseCoverage.ToString());
                }

                if (wordCountMap.ContainsKey(SystemLanguage.ChineseTraditional))
                {
                    var traditionalChineseCoverage = (float)wordCountMap[SystemLanguage.ChineseTraditional] / wordCountMap[SystemLanguage.Japanese];
                    Debug.Log("Traditional chinese coverage : " + traditionalChineseCoverage.ToString());
                }
            }

            if (GUILayout.Button("Verify"))
            {
                var obj = (TableAsset)target;
                obj.Verify();
            }

            DrawDefaultInspector();
        }
    }
}
