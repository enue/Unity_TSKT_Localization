#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using R3;

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
            var k = new LocalizationKey(key);
            k.SubscribeToText(Text);
        }
    }
}
