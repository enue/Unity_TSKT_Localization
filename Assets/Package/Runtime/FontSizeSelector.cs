using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if TSKT_LOCALIZATION_SUPPORT_UNIRX
using UniRx;
#endif

namespace TSKT
{
    [RequireComponent(typeof(Text))]
    public class FontSizeSelector : MonoBehaviour
    {
        Text text;
        Text Text => text ? text : (text = GetComponent<Text>());

        [SerializeField]
        SystemLanguage[] languages = default;

        [SerializeField]
        int[] fontSizes = default;

#if TSKT_LOCALIZATION_SUPPORT_UNIRX
        void Start()
        {
            Localization.currentLanguage.SubscribeWithState((text: Text, initialFontSize: Text.fontSize, fontSizes, languages), (_, _state) =>
            {
                var index = System.Array.IndexOf(_state.languages, _);
                if (index >= 0)
                {
                    _state.text.fontSize = _state.fontSizes[index];
                }
                else
                {
                    _state.text.fontSize = _state.initialFontSize;
                }
            }).AddTo(Text);

        }
#else
        int initialFontSize;

        void Start()
        {
            initialFontSize = Text.fontSize;
            Refresh();
        }

        public void Refresh()
        {
            var index = System.Array.IndexOf(languages, Localization.CurrentLanguage);
            if (index >= 0)
            {
                Text.fontSize = fontSizes[index];
            }
            else
            {
                Text.fontSize = initialFontSize;
            }
        }
#endif
    }
}
