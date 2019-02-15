using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapContinent : NEWHexTileMap
{
    override public void GenerateMap()
    {     
        // Call the base version to generate all the hexes we need
        base.GenerateMap();
        
        int numContinents = 2;
        int continentSpacing = 20;

        for (int c = 0; c < continentSpacing; c++)
        {
            // Make some kind of raised area 
            int numSplats = Random.Range(4, 8); // <----------------------------------------- Amount of "Splats"
            for (int i = 0; i < numSplats; i++)
            {
                int range = Random.Range(5, 8); // <----------------------------------------- Size of Splats 
                int y = Random.Range(range, numRows - range); // <--------------------------- Location of center splat on Rows
                int x = Random.Range(0, 10) - y / 2 + (c * continentSpacing); // <----------- Location of center splat on Columns

                ElevateArea(x, y, range);
            }
        }
        
        UpdateHexVisuals();
    }

    void ElevateArea(int q, int r, int range, float centerHeight = 1f)
    {
        Hex centerHex = GetHexAt(q, r);

        Hex[] areaHexes = GetHexesWithinRangeOf(centerHex, range);

        foreach (Hex h in areaHexes)
        {
//            if (h.Elevation < 0)
//            {
//                h.Elevation = 0;
//            }
            h.Elevation += centerHeight * Mathf.Lerp(1f, 0.25f, Hex.Distance(centerHex, h) / range);
        }
    }   
}
