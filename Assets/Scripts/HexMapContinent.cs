using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapContinent : NEWHexTileMap
{
    override public void GenerateMap()
    {    
        base.GenerateMap();
        
        int numContinents = 4; // <--------------------------------------------------------- Number of continents
        int continentSpacing = numColumns / numContinents; // < ---------------------------------------------------- Spacing of continents

        Random.InitState(4); // seed generator
        
        for (int c = 0; c < numContinents; c++)
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

        // Perlin Noise
        float noiseResolution = 0.06f; // <---------------------------------------------------- Larger values increase density of heights (ie. larger mountainous areas together)
        Vector2 noiseOffset = new Vector2( Random.Range(0f ,1f), Random.Range(0f, 1f));
        
        float noiseScale = 2f; // <---------------------------------------------------------- Larger values makes more islands (and lakes) 
        
        for (int column = 0; column < numColumns; column++)
        {
            for (int row = 0; row < numRows; row++)
            {
                Hex h = GetHexAt(column, row);
                float n = 
                        Mathf.PerlinNoise(((float) column/Mathf.Max(numColumns, numRows) / noiseResolution) + noiseOffset.x, 
                        ((float) row/Mathf.Max(numColumns, numRows)/ noiseResolution) + noiseOffset.y) 
                        - 0.5f;
                h.Elevation += n * noiseScale;
            }
        }

        // Simulate rainfall/moisture and set plains/grasslands + forest
        noiseResolution = 0.05f; // <---------------------------------------------------- Larger values increase density of biomes (ie. larger forested areas together)
        noiseOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));

        noiseScale = 1.85f; // <---------------------------------------------------------- Larger values makes more islands (and lakes) 

        for (int column = 0; column < numColumns; column++)
        {
            for (int row = 0; row < numRows; row++)
            {
                Hex h = GetHexAt(column, row);
                float n =
                        Mathf.PerlinNoise(((float)column / Mathf.Max(numColumns, numRows) / noiseResolution) + noiseOffset.x,
                        ((float)row / Mathf.Max(numColumns, numRows) / noiseResolution) + noiseOffset.y)
                        - 0.5f;
                h.Moisture += n * noiseScale;
            }
        }

        UpdateHexVisuals();
    }

    void ElevateArea(int q, int r, int range, float centerHeight = 0.8f)
    {
        Hex centerHex = GetHexAt(q, r);

        Hex[] areaHexes = GetHexesWithinRangeOf(centerHex, range);

        foreach (Hex h in areaHexes)
        {
            h.Elevation = centerHeight * Mathf.Lerp(1f, 0.25f, Mathf.Pow(Hex.Distance(centerHex, h) / range, 2f));
        }
    }   
}
