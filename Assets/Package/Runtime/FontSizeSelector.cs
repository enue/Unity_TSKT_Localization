#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace TSKT
{
    [RequireComponent(typeof(Text))]
    public class FontSizeSelector : MonoBehaviour
    {
        Text? text;
        Text Text => text ? text! : (text = GetComponent<Text>());

        [SerializeField]
        SystemLanguage[] languages = default!;

        [SerializeField]
        int[] fontSizes = default!;

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
    }
}
