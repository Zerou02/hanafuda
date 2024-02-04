using System;
using Godot;

public class Card
{
    public Types type;
    public Months month;
    public int day;
    public Card(int month, int day)
    {
        this.month = (Months)month;
        this.day = day;
        if (this.month != Months.None)
        {
            this.type = new Constants().cardLookupTable[month][day];
        }
        else
        {
            this.type = Types.None;
        }
    }

    public static Card GetEmpty()
    {
        var card = new Card((int)Months.None, 0);
        return card;
    }

    public bool isValid()
    {
        return this.type != Types.None;
    }

    public Card clone()
    {
        return new Card((int)this.month, (int)this.day);
    }

    public bool isEqual(Card other)
    {
        return this.month == other.month && this.day == other.day;
    }
    public bool equal(Card other)
    {
        return this.month == other.month && this.day == other.day;
    }
    public static byte[] serialize(Card card)
    {
        var retArr = new byte[Constants.serializedCardLength] { 0 };
        retArr[0] = (byte)((byte)((byte)card.month << 2) | (byte)card.day);
        return retArr;
    }

    public void print()
    {
        GD.Print(this.month + "," + this.day + "," + this.type + ",");
    }
    public static Card deserialize(byte[] bytes)
    {
        if (bytes.Length != Constants.serializedCardLength) { throw new Exception("Wrong Format"); }
        var month = (bytes[0] & 0x3C) >> 2;
        var day = bytes[0] & 0x03;
        return new Card(month, day);
    }
}