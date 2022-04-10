#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if TSKT_LOCALIZATION_SUPPORT_UNIRX && TSKT_LOCALIZATION_SUPPORT_TEXTMESHPRO
using UniRx;

namespace TSKT.Localizations
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TMPro.TMP_Text))]
    public class TMP_LocalizationLabel : MonoBehaviour
    {
        TMPro.TMP_Text? text;
        TMPro.TMP_Text Text => text ? text! : (text = GetComponent<TMPro.TMP_Text>());

        [SerializeField]
        string key = default!;

        void Start()
        {
            Localization.currentLanguage
                .SubscribeWithState2(key, Text, (_lang, _key, _text) => SetText(_text, Localization.Get(_lang, _key)))
                .AddTo(Text);
        }

        static void SetText(TMPro.TMP_Text text, string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                text.SetText(value);
                return;
            }
            var font = text.font;
            if (font.HasCharacters(value, out _, searchFallbacks: true, tryAddCharacter: true))
            {
                text.SetText(value);
            }
            else
            {
                text.SetText(value);
                if (font.atlasPopulationMode == TMPro.AtlasPopulationMode.Dynamic)
                {
                    font.ClearFontAssetData();
                }
                foreach (var it in font.fallbackFontAssetTable)
                {
                    if (it.atlasPopulationMode == TMPro.AtlasPopulationMode.Dynamic)
                    {
                        it.ClearFontAssetData();
                    }
                }
                var targetTexts = Object.FindObjectsOfType<TMPro.TMP_Text>(includeInactive: true);
                foreach (var it in targetTexts)
                {
                    if (it.font == font)
                    {
                        it.ForceMeshUpdate();
                    }
                }
            }
        }
    }
}
#endif
