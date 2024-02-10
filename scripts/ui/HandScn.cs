using System.Collections.Generic;
using Godot;

public partial class HandScn : Node2D
{
    public List<CardScn> cardScns = new List<CardScn>();
    public Rect2 bounds = new Rect2(0, 0, 800, 200);
    InputManager inputManager;
    AnimationManager animationManager;

    public override void _Ready()
    {
        inputManager = GetNode<InputManager>(Constants.inputManagerPath);
        animationManager = GetNode<AnimationManager>(Constants.animationManagerPath);

    }

    public override void _Process(double delta)
    {
    }

    public void removeCard(CardScn cardScn)
    {
        this.cardScns = Utils.removeCardScn(cardScns, cardScn);
        Flexbox.alignLeftAnimated(bounds, this.cardScns, animationManager);
    }
    public void setCardScns(List<CardScn> cardScns)
    {
        foreach (var x in cardScns)
        {
            addCardScn(x);
        }
    }

    public void unselectCards()
    {
        foreach (var x in cardScns)
        {
            x.setSelected(false);
        }
    }
    public void addCardScn(CardScn cardScn)
    {
        Utils.reparentTo(cardScn, this);
        this.cardScns.Add(cardScn);
        cardScn.setCard(cardScn.card);
        Flexbox.alignLeftAnimated(bounds, this.cardScns, animationManager);
    }

    public void setPos(Vector2 pos)
    {
        this.bounds = new Rect2(pos, this.bounds.Size);
    }

    public void setSize(Vector2 size)
    {
        this.bounds = new Rect2(this.bounds.Position, size);
    }

    public void allowInteraction(bool val)
    {
        foreach (var x in cardScns)
        {
            x.setAllowInteraction(val);
        }
    }

    public void highlightCards(List<Card> cards)
    {
        foreach (var x in cardScns)
        {
            x.setValid(false);
            foreach (var y in cards)
            {
                if (x.card.equal(y))
                {
                    x.setValid(true);
                }
            }
        }
    }

    public void unHighlightAll()
    {
        highlightCards(new List<Card>());
    }

    public void setSelected(CardScn cardScn)
    {
        foreach (var x in cardScns)
        {
            x.setSelected(false);
        }
        cardScn.setSelected(true);
    }
}