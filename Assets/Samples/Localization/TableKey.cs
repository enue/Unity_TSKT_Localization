namespace TSKT
{
    public static class TableKey
    {
        public const int Fuga = 0;
        public const int Hoge = 1;
        public const int Piyo = 2;
    }

    public static class AutoLocalizationKey
    {
        /// <summary>
        /// 礼儀が人を作る
        /// </summary>
        public static LocalizationKey Fuga => new LocalizationKey(TableKey.Fuga);
        /// <summary>
        /// 鉄は熱いうちに打て
        /// </summary>
        public static LocalizationKey Hoge => new LocalizationKey(TableKey.Hoge);
        /// <summary>
        /// いま{hour}時{minute}分です
        /// </summary>
        public static LocalizationKey Piyo(LocalizationKey hour, LocalizationKey minute)
        {
            return new LocalizationKey(TableKey.Piyo)
                .Replace("{hour}", hour)
                .Replace("{minute}", minute)
;        }
    }
}
