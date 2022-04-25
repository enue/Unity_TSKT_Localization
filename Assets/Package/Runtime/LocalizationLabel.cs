#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace TSKT.Localizations
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class LocalizationLabel : MonoBehaviour
    {
        Text? text;
        Text Text => text ? text! : (text = GetComponent<Text>());

        [SerializeField]
        string key = default!;

        void Start()
        {
            Localization.currentLanguage
                .SubscribeWithState2(key, Text, (_lang, _key, _text) => _text.text = Localization.Get(_lang, _key))
                .AddTo(Text);
        }
    }
}
