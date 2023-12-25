namespace TileHandlers
{
    public interface ITileHandler
    {
        public void TileDown(Tile _tile){}
        public void TileOver(Tile _tile){}
        public void TileUp(Tile _tile){}
        public void SwapTiles() {}
        public void IsCloseTo(Tile start, Tile end){}
    }
}
