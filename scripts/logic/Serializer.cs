using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public enum CardPosition { Deck, Hand, Open, TableCard }

public struct CardMove
{
    public byte source { get; set; }
    public byte srcIdx { get; set; }
    public byte dest { get; set; }
    public byte destIdx { get; set; }
}

public struct GameConfig
{
    public byte startPlayerId;
    public byte maxTurns;
    public byte startPoints;
}
public class Serializer
{
    public static byte[] serializeCardMove(CardPosition source, int srcIdx, CardPosition dest, int destIdx)
    {
        return new byte[] { (byte)source, (byte)srcIdx, (byte)dest, (byte)destIdx };
    }

    public static CardMove deSerializeCardMove(byte[] bytes)
    {
        return new CardMove
        {
            source = bytes[0],
            srcIdx = bytes[1],
            dest = bytes[2],
            destIdx = bytes[3],
        };
    }

    public static byte[] serializeCards(List<Card> cards)
    {
        var byteList = new List<byte>();
        cards.ForEach(x =>
        {
            foreach (var y in Card.serialize(x))
            {
                byteList.Add(y);
            }
        });
        return byteList.ToArray();
    }

    public static List<Card> deserializeCards(byte[] bytes)
    {
        var cards = new List<Card>();
        foreach (var x in bytes)
        {
            cards.Add(Card.deserialize(new byte[] { x }));
        }
        return cards;
    }

    public static byte[] serializeSet(bool[] set)
    {
        var retArr = new byte[set.Length];
        for (int i = 0; i < set.Length; i++)
        {
            retArr[i] = Utils.boolToByte(set[i]);
        }
        return retArr;
    }

    public static bool[] deserializeSet(byte[] set)
    {
        var retArr = new bool[set.Length];
        for (int i = 0; i < set.Length; i++)
        {
            retArr[i] = Utils.byteToBool(set[i]);
        }
        return retArr;
    }

    public static byte[] serializeGameConfig(int startPlayerId, int maxTurns, int startPoints)
    {
        var retArr = new byte[3];
        retArr[0] = (byte)startPlayerId;
        retArr[1] = (byte)maxTurns;
        retArr[2] = (byte)startPoints;
        return retArr;
    }


    public static GameConfig deserializeGameConfig(byte[] bytes)
    {
        return new GameConfig
        {
            startPlayerId = bytes[0],
            maxTurns = bytes[1],
            startPoints = bytes[2]
        };

    }
}