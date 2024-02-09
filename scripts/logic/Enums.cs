public enum Months { January, February, March, April, May, June, July, August, September, October, November, December, None }
public enum Types { Plain, Scroll, BlueScroll, PoetryScroll, Animal, Butterfly, Deer, Boar, Sake, Moon, CherryBlossom, RainMan, Light, None }
public enum Sets { Plain, Scrolls, Animals, BlueScrolls, PoetryScrolls, InoShikaChou, Tsukimi, Hanami, Sankou, Ameshikou, Shikou, Gokou }
public enum FlexboxModes { LeftAligned };

public enum MessageType
{
    InitGame, SetActivePlayer, InitDeck, MoveCard, MatchTableCard,
    MatchEmptyTableCard, StartDeckTurn, MatchTableCardWithDeck, SwitchPlayer, OpenDeckCard,
    DeckChoose, UiModeSetPlayerTurn, CheckHasSet, DisplayKoiKoiMenu, KoiKoiPressed, PeerConnected,
    StartRound, RoundEnded, ChangePoints, GameEnded, OutOfCards, SetEnemyName, BothLoaded, Quit, Rematch
}