#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
            var k = new LocalizationKey(key);
            k.SubscribeToText(Text);
        }
    }
}
