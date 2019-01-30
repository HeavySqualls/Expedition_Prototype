using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapContinent : NEWHexTileMap
{
    public void GenerateMap()
    {
        // First, call the base version to generate all the hexes we need
        base.GenerateMap();
    }
}
