using System.Collections;
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
        static void Initialzie()
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
            Localization.ForceRefresh();
            Refresh();
        }

        public void OnClickedEnglishButton()
        {
            Localization.CurrentLanguage = SystemLanguage.English;
            Localization.ForceRefresh();
            Refresh();
        }

        void Refresh()
        {
            label.text = Localization.Get(TableKey.Fuga);

            var now = System.DateTime.Now;
            var key = new LocalizationKey("Piyo",
                ("{hour}", LocalizationKey.CreateRaw(now.Hour.ToString())),
                ("{minute}", LocalizationKey.CreateRaw(now.Minute.ToString())));
            timeLabel.text = key.Localize();
        }
    }
}
