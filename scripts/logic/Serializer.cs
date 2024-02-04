using System.Collections.Generic;
using System.Linq;

public enum CardPosition { Deck, Hand, Open, TableCard }

public struct CardMove
{
    public byte source { get; set; }
    public byte srcIdx { get; set; }
    public byte dest { get; set; }
    public byte destIdx { get; set; }
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
}