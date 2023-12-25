using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TileHandlers;

public class Board : MonoBehaviour, ITileHandler, IMatchHandler
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

    void Start()
    {
        // initialX = -width / 2;
        // initialY = -height / 2;
        
        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(Initialize());
    }

    #region Setup Board Parts

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

    #endregion

    #region Initialize Board

    private void InitializeBoardParts()
    {
        Tiles = new Tile[width, height];
        Pieces = new Piece[width, height];
    }
    
    private void InitializeCamera()
    {
        CameraEvents.Current.PositionCamera((float)(width / 2) - .5f, (float)(height / 2) - .5f);

        float horizontal = width + 1;
        float vertical = (height / 2) + 1;

        CameraEvents.Current.ChangeOrtographicSize(horizontal,vertical);
    } 

    IEnumerator Initialize()
    {
        InitializeBoardParts();

        SetupBoard();
        yield return null;

        InitializeCamera();

        SetupPieces();
        
        currentCoroutine = null;
    }
    
    #endregion

    #region ITileHandler

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
            currentCoroutine = StartCoroutine( SwapTiles());
        }
    }

    IEnumerator SwapTiles()
    {
        Piece startPiece = Pieces[startTile.X, startTile.Y];
        Piece endPiece = Pieces[endTile.X, endTile.Y];

        startPiece.Move(endTile.X, endTile.Y);
        endPiece.Move(startTile.X, startTile.Y);

        Pieces[startTile.X, startTile.Y] = endPiece;
        Pieces[endTile.X, endTile.Y] = startPiece;

        yield return new WaitForSeconds(0.6f);

        bool foundMatch = false;

        List<Piece> startMatches = GetMatchByPiece(startTile.X, startTile.Y, 3);
        List<Piece> endMatches = GetMatchByPiece(endTile.X, endTile.Y, 3);
        
        startMatches.ForEach(piece =>
        {
            foundMatch = true;
            Pieces[piece.X, piece.Y] = null;
            Destroy(piece.gameObject);
        });
        
        endMatches.ForEach(piece =>
        {
            foundMatch = true;
            Pieces[piece.X, piece.Y] = null;
            Destroy(piece.gameObject);
        });

        if (!foundMatch)
        {
            startPiece.Move(startTile.X, startTile.Y);
            endPiece.Move(endTile.X, endTile.Y);

            Pieces[startTile.X, startTile.Y] = startPiece;
            Pieces[endTile.X, endTile.Y] = endPiece;
        }
        
        startTile = null;
        endTile = null;
        swappingPieces = false;
        
        yield return null;

        currentCoroutine = null;
    }

    public bool IsCloseTo(Tile start, Tile end)
    {
        bool horizontalMoveChecking = Math.Abs(start.X - end.X) == 1 && start.Y == end.Y;
        bool verticalMoveChecking = Math.Abs(start.Y - end.Y) == 1 && start.X == end.X;

        // return horizontalMoveChecking || verticalMoveChecking;
        if (horizontalMoveChecking)
            return true;
        if (verticalMoveChecking)
            return true;

        return false;
    }

    #endregion

    #region IMatchHandler

    public List<Piece> GetMatchByDirection(int xpos, int ypos, Vector2 direction, int minPieces = 3)
    {
        List<Piece> matches = new List<Piece>();
        Piece startPiece = Pieces[xpos, ypos];
        matches.Add(startPiece);

        int nextX;
        int nextY;
        int maxVal = width > height ? width : height;

        for (int i = 1; i < maxVal; i++)
        {
            nextX = xpos + ((int)direction.x * i);
            nextY = ypos + ((int)direction.y * i);
            if (nextX >= 0 && nextX < width && nextY >= 0 && nextY < height)
            {
                Piece nextPiece = Pieces[nextX, nextY];
                if (nextPiece != null && nextPiece.pieceType == startPiece.pieceType)
                {
                    matches.Add(nextPiece);
                }
                else
                {
                    break;
                }
            }
        }
        
        return matches.Count >= minPieces ? matches : null;
    }

    public List<Piece> GetMatchByPiece(int xpos, int ypos, int minPieces = 3)
    {
        List<Piece> upMatches = GetMatchByDirection(xpos, ypos, Vector2.up, 2);
        List<Piece> downMatches = GetMatchByDirection(xpos, ypos, Vector2.down, 2);
        List<Piece> leftMatches = GetMatchByDirection(xpos, ypos, Vector2.left, 2);
        List<Piece> rightMatches = GetMatchByDirection(xpos, ypos, Vector2.right, 2);

        upMatches = upMatches.ResetIfNull();
        downMatches = downMatches.ResetIfNull();
        leftMatches = leftMatches.ResetIfNull();
        rightMatches = rightMatches.ResetIfNull();

        List<Piece> verticalMatches = upMatches.Union(downMatches).ToList();
        List<Piece> horizontalMatches = leftMatches.Union(rightMatches).ToList();

        List<Piece> foundMatches = new List<Piece>();

        if (verticalMatches.Count >= minPieces)
        {
            foundMatches = foundMatches.Union(verticalMatches).ToList();
        }

        if (horizontalMatches.Count >= minPieces)
        {
            foundMatches = foundMatches.Union(horizontalMatches).ToList();
        }

        return foundMatches;
    }

    #endregion
}
