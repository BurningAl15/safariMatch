namespace TileHandlers
{
    public interface ITileHandler
    {
        public void TileDown(Tile _tile){}
        public void TileOver(Tile _tile){}
        public void TileUp(Tile _tile){}
        public void SwapTiles() {}
        public void IsCloseTo(Tile start, Tile end){}
        public bool HasPreviousMatches(int xpos, int ypos) => false;
        public void ClearPieceAt(int xpos, int ypos){}
        public void ClearPieceAt(Piece piece){}

    }
}
