using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class SpriteManager : MonoSingleton<SpriteManager>
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private GenericDictionary<PieceType, Sprite> pieceDictionary = new GenericDictionary<PieceType, Sprite>();

    protected override void Awake()
    {
        Initialize();
        base.Awake();
    }

    public void Initialize()
    {
        sprites = Resources.LoadAll<Sprite>("Sprites/Animals");
        
        pieceDictionary.Setup(null);
       
        for (int i = 0; i < sprites.Length; i++)
        {
            pieceDictionary.Add((PieceType)i,sprites[i]);
        }
    }

    public Sprite GetSprite(PieceType type)
    {
        return pieceDictionary.Get(type);
    }
}
