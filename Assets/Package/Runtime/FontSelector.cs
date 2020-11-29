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
    public class FontSelector : MonoBehaviour
    {
        Text text;
        Text Text => text ? text : (text = GetComponent<Text>());

        [SerializeField]
        SystemLanguage[] languages = default;

        [SerializeField]
        Font[] fonts = default;

#if TSKT_LOCALIZATION_SUPPORT_UNIRX
        void Start()
        {
            Localization.currentLanguage.SubscribeWithState((text: Text, initialFont: Text.font, fonts, languages), (_, _state) =>
            {
                var index = System.Array.IndexOf(_state.languages, _);
                if (index >= 0)
                {
                    _state.text.font = _state.fonts[index];
                }
                else
                {
                    _state.text.font = _state.initialFont;
                }
            }).AddTo(Text);

        }
#else
        Font initialFont;

        void Start()
        {
            initialFont = Text.font;
            Refresh();
        }

        public void Refresh()
        {
            var index = System.Array.IndexOf(languages, Localization.CurrentLanguage);
            if (index >= 0)
            {
                Text.font = fonts[index];
            }
            else
            {
                Text.font = initialFont;
            }
        }
#endif
    }
}
