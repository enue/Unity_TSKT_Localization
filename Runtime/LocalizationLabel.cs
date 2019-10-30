using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TSKT.Localizations
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class LocalizationLabel : MonoBehaviour
    {
        Text text;
        Text Text => text ?? (text = GetComponent<Text>());

        [SerializeField]
        string key = default;

        void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            Text.text = Localization.Get(key);
        }
    }
}
