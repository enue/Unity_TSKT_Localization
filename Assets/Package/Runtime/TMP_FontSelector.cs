#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using R3;
using Cysharp.Threading.Tasks;
using TMPro;

namespace TSKT
{
    [RequireComponent(typeof(TMPro.TMP_Text))]
    public class TMP_FontSelector : MonoBehaviour
    {
        [System.Serializable]
        struct Style
        {
            public SystemLanguage language;

            [SerializeField]
            public TMP_FontAsset font;

            [SerializeField]
            public Material? material;
        }

        TMPro.TMP_Text? text;
        TMPro.TMP_Text Text => text ? text! : (text = GetComponent<TMPro.TMP_Text>());

        [SerializeField]
        Style[] styles = default!;

        void Start()
        {
            Localization.currentLanguage.Subscribe((initialFont: Text.font, initialMaterial: Text.fontSharedMaterial), (_, _state) =>
            {
                var index = System.Array.FindIndex(styles, style => style.language == _);
                if (index >= 0)
                {
                    var style = styles[index];
                    Text.font = style.font;
                    if (style.material)
                    {
                        Text.fontMaterial = style.material;
                    }
                }
                else
                {
                    Text.font = _state.initialFont;
                    Text.fontMaterial = _state.initialMaterial;
                }
            }).AddTo(Text);
        }
    }
}
