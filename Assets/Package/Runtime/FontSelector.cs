using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    }
}
