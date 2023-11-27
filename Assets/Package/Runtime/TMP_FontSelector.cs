#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using TMPro;

namespace TSKT
{
    [RequireComponent(typeof(TMPro.TMP_Text))]
    public class TMP_FontSelector : MonoBehaviour
    {
        TMPro.TMP_Text? text;
        TMPro.TMP_Text Text => text ? text! : (text = GetComponent<TMPro.TMP_Text>());

        [SerializeField]
        SystemLanguage[] languages = default!;

        [SerializeField]
        TMP_FontAsset?[] fonts = default!;

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
            }).AddTo(Text.destroyCancellationToken);

        }
    }
}
