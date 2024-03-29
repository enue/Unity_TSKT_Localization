﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TSKT
{
    public class LocalizationSample : MonoBehaviour
    {
        [SerializeField]
        Text label = default;

        [SerializeField]
        Text timeLabel = default;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            Localization.CurrentLanguage = SystemLanguage.Japanese;
            Localization.SetTable(Resources.Load<Localizations.TableAsset>("TableAsset").Table);
        }

        void Start()
        {
            Refresh();
        }

        public void OnClickedJapaneseButton()
        {
            Localization.CurrentLanguage = SystemLanguage.Japanese;
            Refresh();
        }

        public void OnClickedEnglishButton()
        {
            Localization.CurrentLanguage = SystemLanguage.English;
            Refresh();
        }

        void Refresh()
        {
            label.text = Localization.Get(TableKey.Fuga);

            var now = System.DateTime.Now;
            var key = new LocalizationKey("Piyo").Replace(
                ("{hour}", LocalizationKey.CreateRaw(now.Hour.ToString())),
                ("{minute}", LocalizationKey.CreateRaw(now.Minute.ToString())));
            timeLabel.text = key.Localize();
        }
    }
}
