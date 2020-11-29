using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if TSKT_LOCALIZATION_SUPPORT_UNIRX
using UniRx;
#endif

namespace TSKT.Localizations
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class LocalizationLabel : MonoBehaviour
    {
        Text text;
        Text Text => text ? text : (text = GetComponent<Text>());

        [SerializeField]
        string key = default;

#if TSKT_LOCALIZATION_SUPPORT_UNIRX
        void Start()
        {
            Localization.currentLanguage
                .SubscribeWithState2(key, Text, (_lang, _key, _text) => _text.text = Localization.Get(_key))
                .AddTo(Text);
        }
#else
        void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (Localization.Languages == null)
                {
                    return;
                }
            }
#endif
            Refresh();
        }

        public void Refresh()
        {
            Text.text = Localization.Get(key);
        }
#endif
    }
}
