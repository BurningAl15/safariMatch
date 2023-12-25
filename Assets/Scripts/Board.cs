using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using input;

public class Board : MonoBehaviour, ITileHandler
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float initialX;
    [SerializeField] private float initialY;


    [SerializeField] private GameObject tileObject;
    public GameObject[] availablePieces;

    [SerializeField] private Tile[,] Tiles;
    [SerializeField] private Piece[,] Pieces;
    
    Coroutine currentCoroutine = null;

    [SerializeField] private Tile startTile;
    [SerializeField] private Tile endTile;

    private bool swappingPieces = false;
    
    private void InitializeBoard()
    {
        Tiles = new Tile[width, height];
        Pieces = new Piece[width, height];
    }

    // Start is called before the first frame update
    void Start()
    {
        // initialX = -width / 2;
        // initialY = -height / 2;
        
        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(Initialize());
    }
    
    private void SetupPieces()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject selectedPieces = availablePieces[UnityEngine.Random.Range(0, availablePieces.Length)];
                GameObject obj = Instantiate(selectedPieces, new Vector3(x, y, -5), Quaternion.identity);
                obj.transform.parent = this.transform;
                Pieces[x, y] = obj.GetComponent<Piece>();
                Pieces[x, y]?.Setup(x, y, this);
            }
        }
    }
    
    private void SetupBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject obj = Instantiate(tileObject, new Vector3(x, y, -5), Quaternion.identity);
                obj.transform.parent = this.transform;
                Tiles[x, y] = obj.GetComponent<Tile>();
                Tiles[x, y]?.Setup(x, y, this);
            }
        }
    }
    
    IEnumerator Initialize()
    {
        InitializeBoard();

        SetupBoard();
        yield return null;
        CameraEvents.Current.PositionCamera((float)(width / 2) - .5f, (float)(height / 2) - .5f);

        float horizontal = width + 1;
        float vertical = (height / 2) + 1;

        CameraEvents.Current.ChangeOrtographicSize(horizontal,vertical);

        SetupPieces();
        
        currentCoroutine = null;
    }

    public void TileDown(Tile _tile)
    {
        startTile = _tile;
    }

    public void TileOver(Tile _tile)
    {
        endTile = _tile;
    }

    public void TileUp(Tile _tile)
    {
        if (startTile != null && endTile != null && IsCloseTo(startTile, endTile))
        {
           StartCoroutine( SwapTiles());
        }
    }

    IEnumerator SwapTiles()
    {
        Piece startPiece = Pieces[startTile.x, startTile.y];
        Piece endPiece = Pieces[endTile.x, endTile.y];

        startPiece.Move(endTile.x, endTile.y);
        endPiece.Move(startTile.x, startTile.y);

        Pieces[startTile.x, startTile.y] = endPiece;
        Pieces[endTile.x, endTile.y] = startPiece;

        yield return new WaitForSeconds(0.6f);
        
        startTile = null;
        endTile = null;
        swappingPieces = false;
        
        yield return null;
    }

    public bool IsCloseTo(Tile start, Tile end)
    {
        bool horizontalMoveChecking = Math.Abs(start.x - end.x) == 1 && start.y == end.y;
        bool verticalMoveChecking = Math.Abs(start.y - end.y) == 1 && start.x == end.x;

        // return horizontalMoveChecking || verticalMoveChecking;
        if (horizontalMoveChecking)
            return true;
        if (verticalMoveChecking)
            return true;

        return false;
    }
}
