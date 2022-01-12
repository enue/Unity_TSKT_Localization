namespace TSKT
{
    public static class TableKey
    {
        public const int Foo = 0;
        public const int Fuga = 1;
        public const int Hoge = 2;
        public const int Piyo = 3;
    }

    public static class AutoLocalizationKey
    {
        /// <summary>
        /// いま{hour}
        /// </summary>
        public static LocalizationKey Foo(LocalizationKey hour)
        {
            return new LocalizationKey(TableKey.Foo)
                .Replace("{hour}", hour);
        }
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
                .Replace(("{hour}", hour), ("{minute}", minute));
        }
    }
}
