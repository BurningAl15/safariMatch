using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileHandlers
{
    public interface IMatchHandler
    {
        public List<Piece> GetMatchByDirection(int xpos, int ypos, Vector2 direction, int minPieces = 3) => null;

        public List<Piece> GetMatchByPiece(int xpos, int ypos, int minPieces = 3) => null;
    }
}

