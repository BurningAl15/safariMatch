using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace input
{
    public interface ITileHandler
    {
        public void TileDown(Tile _tile){}
        public void TileOver(Tile _tile){}
        public void TileUp(Tile _tile){}

        public void SwapTiles() {}
    }
}
