using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

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

    private void Awake() 
    {
        Setup(0, 0, null);
    }

    public void Setup(int _x, int _y, Board _board)
    {
        pieceSprite.sprite =  SpriteManager.Current.GetSprite(pieceType);
        x = _x;
        y = _y;
        board = _board;
    }

    public void Move(int desX, int desY)
    {
        transform.DOMove(new Vector3(desX, desY, -5f),0.25f).SetEase(Ease.InOutCubic).onComplete = () =>
        {
            x = desX;
            y = desY;
        };
    }

    [ContextMenu("Test Move")]
    public void MoveTest()
    {
        Move(0,0);
    }
}
