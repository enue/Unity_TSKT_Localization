# Unty_TSKT_Localization

Packages/manifest.json

```json
{
  "dependencies": {
    "com.github.enue.tskt_localization": "https://github.com/enue/Unity_TSKT_Localization.git",
    "com.github.enue.tskt_container": "https://github.com/enue/Unity_TSKT_Container.git",
  }
}
```

## 初期化

```cs
public class Hoge
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        Localization.Create(Load, SystemLanguage.Japanese);
    }

    static TableAsset Load()
    {
        return Resources.Load<TableAsset>("TableAsset");
    }
}
```

## 翻訳辞書Json

### 構造

```json
{
  "items": [
    {
      "key": "Element.物理",
      "pairs": [
        {
          "language": "Japanese",
          "text": "物理"
        },
        {
          "language": "English",
          "text": "Physical"
        },
      ]
    },
    {
      "key": "Element.魔法",
      "pairs": [
        {
          "language": "Japanese",
          "text": "魔法"
        },
        {
          "language": "English",
          "text": "Magic"
        },
      ]
    },
  ]
}
```

### 置き場所

Assetsと同じ階層にLocalizationフォルダを作る

```
Project folder
├Assets
└Localization
 ├Hoge.json
 ├Fuga.json
 └Piyo.json
```

### Excelからの変換ツール

https://github.com/enue/game22_localization_tool

