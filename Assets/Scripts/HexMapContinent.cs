using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapContinent : NEWHexTileMap
{
    override public void GenerateMap()
    {
        // First, call the base version to generate all the hexes we need
        base.GenerateMap();
        
        // Make some kind of raised area 
        ElevateArea(25, 19, 6);
        ElevateArea(24, 29, 6);
        ElevateArea(14, 22, 6);

        // We want to elevate some hexes. How do we access?
        // Set hex that we want? 
        // Set height of hex? 

        // Add lumpiness (Perlin Noise??)

        // Set mesh to mountain/hill/flat/water based on height

        // Simulate rainfall/moisture and set plains/grasslands + forest
        
        // Now make sure all the hex visuals are updated to match the hex data
        
        UpdateHexVisuals();
    }

    void ElevateArea(int q, int r, int range, float centerHeight = 0.5f)
    {
        Hex centerHex = GetHexAt(q, r);

        // centerHex.Elevation = 0.5f;

        Hex[] areaHexes = GetHexesWithinRangeOf(centerHex, range);

        foreach (Hex h in areaHexes)
        {
            if (h.Elevation < 0)
            {
                h.Elevation = 0;
            }
            h.Elevation += centerHeight * Mathf.Lerp(1f, 0.25f, Hex.Distance(centerHex, h) / range);
        }
    }
    
}
