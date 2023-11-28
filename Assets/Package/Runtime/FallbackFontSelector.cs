#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;
using Cysharp.Threading.Tasks;

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
        LanguageFontPair[] pairs = default!;

        void Start()
        {
            var defaultFont = TMP_Settings.fallbackFontAssets[destinationIndex];
            Localization.currentLanguage.SubscribeWithState(defaultFont, (language, _defaultFont) =>
            {
                var index = System.Array.FindIndex(pairs, _ => _.language == language);

                if (index >= 0)
                {
                    TMP_Settings.fallbackFontAssets[destinationIndex] = pairs[index].font;
                }
                else
                {
                    TMP_Settings.fallbackFontAssets[destinationIndex] = _defaultFont;
                }
            }).AddTo(destroyCancellationToken);
        }
    }
}
