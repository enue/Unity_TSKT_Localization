#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using R3;
using TMPro;

namespace TSKT
{
    public class FallbackFontSelector : MonoBehaviour
    {
        [System.Serializable]
        struct LanguageFontPair
        {
            public SystemLanguage language;
            public TMP_FontAsset font;
        }

        [SerializeField]
        int destinationIndex;

        [SerializeField]
        TMP_FontAsset defaultFont = default!;

        [SerializeField]
        LanguageFontPair[] pairs = default!;

        void Start()
        {
            Localization.currentLanguage.Subscribe(language =>
            {
                var index = System.Array.FindIndex(pairs, _ => _.language == language);

                if (index >= 0)
                {
                    TMP_Settings.fallbackFontAssets[destinationIndex] = pairs[index].font;
                }
                else
                {
                    TMP_Settings.fallbackFontAssets[destinationIndex] = defaultFont;
                }
            }).AddTo(this);
        }
    }
}
