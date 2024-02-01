public class Constants
{
    public readonly Types[][] cardLookupTable = new Types[][] { new Types[] { Types.Plain, Types.Plain, Types.PoetryScroll, Types.Light },
                                                                new Types[] { Types.Plain, Types.Plain, Types.PoetryScroll, Types.Animal },
                                                                new Types[] { Types.Plain, Types.Plain, Types.PoetryScroll, Types.CherryBlossom },
                                                                new Types[] { Types.Plain, Types.Plain, Types.Scroll, Types.Animal },
                                                                new Types[] { Types.Plain, Types.Plain, Types.Scroll, Types.Animal },
                                                                new Types[] { Types.Plain, Types.Plain, Types.BlueScroll, Types.Butterfly },
                                                                new Types[] { Types.Plain, Types.Plain, Types.Scroll, Types.Boar },
                                                                new Types[] { Types.Plain, Types.Plain, Types.Animal, Types.Moon },
                                                                new Types[] { Types.Plain, Types.Plain, Types.BlueScroll, Types.Sake },
                                                                new Types[] { Types.Plain, Types.Plain, Types.BlueScroll, Types.Deer },
                                                                new Types[] { Types.Plain, Types.Scroll, Types.Animal, Types.RainMan },
                                                                new Types[] { Types.Plain, Types.Plain, Types.Plain, Types.Light }};

    public const int cardWidth = 30;
    public const int cardHeight = 48;
    public const string inputManagerPath = "/root/InputManager";
}