#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace TSKT
{
    [RequireComponent(typeof(Text))]
    public class FontSelector : MonoBehaviour
    {
        Text? text;
        Text Text => text ? text! : (text = GetComponent<Text>());

        [SerializeField]
        SystemLanguage[] languages = default!;

        [SerializeField]
        Font?[] fonts = default!;

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
    }
}
