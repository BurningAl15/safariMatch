using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum PieceType
{
    elephant,
    giraffe,
    hippo,
    monkey,
    panda,
    parrot,
    penguin,
    pig,
    rabbit,
    snake
};

public class Piece : MonoBehaviour
{
    public int X => x;

    public int Y => y;

    [SerializeField] private int x;

    [SerializeField] private int y;

    [SerializeField] private Board board;

    [SerializeField] private SpriteRenderer pieceSprite;

    public PieceType pieceType;
    private bool hasFinished = true;
    
    public void Setup(int _x, int _y, Board _board)
    {
        pieceSprite.sprite =  SpriteManager.Current.GetSprite(pieceType);
        x = _x;
        y = _y;
        board = _board;
        StartCoroutine(ReboundAnimation(.5f));
    }

    public void Move(int desX, int desY)
    {
        transform.DOMove(new Vector3(desX, desY, -5f),0.25f).SetEase(Ease.InOutCubic).onComplete = () =>
        {
            x = desX;
            y = desY;
        };
    }

    public IEnumerator DestroyPiece()
    {
        yield return StartCoroutine(ReboundAnimation());
        yield return new WaitForSeconds(.2f);
        yield return new WaitUntil(()=>hasFinished);
        Destroy(this.gameObject);
    }

    IEnumerator ReboundAnimation(float timeToWait=0)
    {
        yield return new WaitForSeconds(timeToWait);
        hasFinished = false;
        
        Sequence sequence = DOTween.Sequence();
        
        sequence.Append(transform.DOScale(AnimationUtils.reboundAnimationSettings[0].endValue, AnimationUtils.reboundAnimationSettings[0].duration).SetEase(AnimationUtils.reboundAnimationSettings[0].ease));
        sequence.Join(transform.DOScale(AnimationUtils.reboundAnimationSettings[1].endValue, AnimationUtils.reboundAnimationSettings[1].duration).SetEase(AnimationUtils.reboundAnimationSettings[1].ease).SetDelay(AnimationUtils.reboundAnimationSettings[1].delay));
        sequence.Join(transform.DOScale(AnimationUtils.reboundAnimationSettings[2].endValue, AnimationUtils.reboundAnimationSettings[2].duration).SetEase(AnimationUtils.reboundAnimationSettings[2].ease).SetDelay(AnimationUtils.reboundAnimationSettings[2].delay));
        sequence.Join(transform.DOScale(AnimationUtils.reboundAnimationSettings[3].endValue, AnimationUtils.reboundAnimationSettings[3].duration).SetEase(AnimationUtils.reboundAnimationSettings[3].ease).SetDelay(AnimationUtils.reboundAnimationSettings[3].delay)).onComplete = () =>
        {
            hasFinished = true;
        };
        yield return null;
    }

    [ContextMenu("Test Move")]
    public void MoveTest()
    {
        Move(0,0);
    }
}
