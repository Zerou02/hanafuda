using System;
using Godot;

public class Card
{
    public Types type;
    public Months month;
    public int day;
    public bool valid;
    public Card(int month, int day)
    {
        this.month = (Months)month;
        this.day = day;
        this.type = new Constants().cardLookupTable[month][day];
        this.valid = true;
    }

    public static Card GetEmpty()
    {
        var card = new Card(0, 0);
        card.valid = false;
        return card;
    }

    public bool isValid()
    {
        return this.valid;
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
        var retArr = new byte[3];
        retArr[0] = (byte)card.month;
        retArr[1] = (byte)card.day;
        retArr[2] = Utils.boolToByte(card.valid);
        return retArr;
    }

    public void print()
    {
        GD.Print(this.month + "," + this.day + "," + this.type + "," + this.valid);
    }
    public static Card deserialize(byte[] bytes)
    {
        if (bytes.Length != 3) { throw new Exception("Wrong Format"); }
        var month = (int)bytes[0];
        var day = (int)bytes[1];
        var valid = Utils.byteToBool(bytes[2]);
        return valid ? new Card(month, day) : Card.GetEmpty();
    }
}