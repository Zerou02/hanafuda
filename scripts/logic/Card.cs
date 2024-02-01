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

    public bool equal(Card other)
    {
        return this.month == other.month && this.day == other.day;
    }
}