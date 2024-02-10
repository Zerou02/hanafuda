using System.Collections.Generic;
using Godot;

public abstract class Animation
{
    public abstract float duration { get; }
    public abstract Animations type { get; }
}

public class OpenDeckAnimation : Animation
{
    public override float duration => 0.5f;
    public override Animations type => Animations.OpenDeck;
}

public class MoveCardAnimation : Animation
{
    public override float duration => 0.5f;
    public override Animations type => Animations.MoveCard;
    public CardScn cardScn;
    public Vector2 to;
    public MoveCardAnimation(CardScn cardScn, Vector2 to)
    {
        this.cardScn = cardScn;
        this.to = to;
    }
};

public enum Animations { OpenDeck, MoveCard };
public partial class AnimationManager : Node
{
    public UiManager uiManager;
    public Queue<Animation> animationQueue = new Queue<Animation>();
    public bool isInAnimation = false;
    bool animationFinished = true;
    public override void _Process(double delta)
    {
        if (animationFinished)
        {
            if (animationQueue.Count == 0)
            {
                isInAnimation = false;
            }
            else
            {
                var anim = animationQueue.Dequeue();
                playAnimation(anim);
            }
        }
    }

    public void addAnimation(Animation animation)
    {
        animationQueue.Enqueue(animation);
    }
    void playAnimation(Animation animation)
    {
        isInAnimation = true;
        animationFinished = false;
        if (animation.type == Animations.OpenDeck)
        {
            GetTree().CreateTimer(animation.duration).Timeout += () => { animationFinished = true; };
            return;
        }
        else if (animation.type == Animations.MoveCard)
        {
            var movAnim = (MoveCardAnimation)animation;
            movAnim.cardScn.animateMove(movAnim.to, movAnim.duration);
        }
        animationFinished = true;
    }
}