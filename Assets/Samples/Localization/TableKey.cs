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
        public static LocalizationKey Fuga => new LocalizationKey(TableKey.Fuga);
        public static LocalizationKey Hoge => new LocalizationKey(TableKey.Hoge);
        public static LocalizationKey Piyo => new LocalizationKey(TableKey.Piyo);
    }
}
