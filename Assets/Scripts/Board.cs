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

    private List<bool> validatePieceDestruction = new List<bool>();
    
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
        int maxIterations = 50;
        int currentIteration = 0;
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                currentIteration = 0;
                Piece newPiece = CreatePieceAt(x,y);
                while (HasPreviousMatches(x, y))
                {
                    ClearPieceAt(x, y);
                    newPiece = CreatePieceAt(x, y);
                    currentIteration++;
                    if (currentIteration > maxIterations)
                    {
                        break;
                    }
                }
            }
        }
    }

    private Piece CreatePieceAt(int x, int y)
    {
        GameObject selectedPieces = availablePieces[UnityEngine.Random.Range(0, availablePieces.Length)];
        GameObject obj = Instantiate(selectedPieces, new Vector3(x, y, -5), Quaternion.identity);
        obj.transform.parent = this.transform;
        Pieces[x, y] = obj.GetComponent<Piece>();
        Pieces[x, y]?.Setup(x, y, this);
        return Pieces[x, y];
    }
    
    private void SetupBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile newTile = CreateTileAt(x, y);
            }
        }
    }

    private Tile CreateTileAt(int x, int y)
    {
        GameObject obj = Instantiate(tileObject, new Vector3(x, y, -5), Quaternion.identity);
        obj.transform.parent = this.transform;
        Tiles[x, y] = obj.GetComponent<Tile>();
        Tiles[x, y]?.Setup(x, y, this);
        return Tiles[x, y];
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
        
        List<Piece> startMatches = GetMatchByPiece(startTile.X, startTile.Y, 3);
        List<Piece> endMatches = GetMatchByPiece(endTile.X, endTile.Y, 3);

        List<Piece> allMatches = startMatches.Union(endMatches).ToList();
        
     
        if (allMatches.Count == 0)
        {
            startPiece.Move(startTile.X, startTile.Y);
            endPiece.Move(endTile.X, endTile.Y);

            Pieces[startTile.X, startTile.Y] = startPiece;
            Pieces[endTile.X, endTile.Y] = endPiece;
        }
        else
        {
            ClearPieces(allMatches);
        }
        
        startTile = null;
        endTile = null;
        swappingPieces = false;
        
        yield return null;
        currentCoroutine = null;
    }
    
    private void ClearPieces(List<Piece> piecesToClear)
    {
        int index = 0;
        validatePieceDestruction = new List<bool>();

        for (int i = 0; i < piecesToClear.Count; i++)
        {
            validatePieceDestruction.Add(false);
        }
        
        piecesToClear.ForEach(piece =>
        {
            ClearPieceAt(piece, index);
            index++;
        });

        StartCoroutine(CollapseColumnsEffect(piecesToClear));
    }

    private List<int> GetColumns(List<Piece> piecesToClear)
    {
        var result = new List<int>();

        piecesToClear.ForEach(piece =>
        {
            if (!result.Contains(piece.X))
            {
                result.Add(piece.X);
            }
        });

        return result;
    }

    private List<Piece> CollapseColumns(List<int> columns, float timeToCollapse)
    {
        List<Piece> movingPieces = new List<Piece>();

        for (int i = 0; i < columns.Count; i++)
        {
            var column = columns[i];
            for(int y = 0; y < height; y++)
            {
                if(Pieces[column, y] == null)
                {
                    for(int yplus = y +1; yplus<height; yplus++)
                    {
                        if (Pieces[column, yplus] != null)
                        {
                            Pieces[column, yplus].Move(column, y);
                            Pieces[column, y] = Pieces[column, yplus];
                            if(!movingPieces.Contains(Pieces[column, y]))
                            {
                                movingPieces.Add(Pieces[column, y]);
                            }
                            Pieces[column, yplus] = null;
                            break;
                        }
                    }
                }
            }
        }
        return movingPieces;
    }

    IEnumerator CollapseColumnsEffect(List<Piece> piecesToClear)
    {
        yield return new WaitUntil(() => validatePieceDestruction.All(value => value));
        yield return new WaitForSeconds(0.1f);
        List<int> columns = GetColumns(piecesToClear);
        List<Piece> collapsedPieces =  CollapseColumns(columns, 0.3f);
    }
    
    public bool IsCloseTo(Tile start, Tile end)
    {
        bool horizontalMoveChecking = Math.Abs(start.X - end.X) == 1 && start.Y == end.Y;
        bool verticalMoveChecking = Math.Abs(start.Y - end.Y) == 1 && start.X == end.X;

        if (horizontalMoveChecking)
            return true;
        if (verticalMoveChecking)
            return true;

        return false;
    }

    public bool HasPreviousMatches(int xpos, int ypos)
    {
        List<Piece> downMatches = GetMatchByDirection(xpos, ypos, Vector2.down, 2);
        List<Piece> leftMatches = GetMatchByDirection(xpos, ypos, Vector2.left, 2);
        
        downMatches = downMatches.ResetIfNull();
        leftMatches = leftMatches.ResetIfNull();

        return (downMatches.Count > 0 || leftMatches.Count > 0);
    }

    public void ClearPieceAt(int xpos, int ypos)
    {
        Piece piece = Pieces[xpos, ypos];
        Destroy(piece.gameObject);
        Pieces[xpos, ypos] = null;
    }

    public void ClearPieceAt(Piece piece, int index)
    {
        Pieces[piece.X, piece.Y] = null;
        StartCoroutine(ValidatePieceClearing(piece, index));
    }

    IEnumerator ValidatePieceClearing(Piece piece, int index)
    {
        yield return StartCoroutine(piece.DestroyPiece());
        validatePieceDestruction[index] = true;
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
