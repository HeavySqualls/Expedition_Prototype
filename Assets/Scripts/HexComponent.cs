using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexComponent : MonoBehaviour
{
    public Hex Hex;
    public NEWHexTileMap NEWHexTileMap;
    
    public void UpdatePosition()
    {
        this.transform.position = Hex.PositionFromCamera(Camera.main.transform.position, NEWHexTileMap.numRows, NEWHexTileMap.numColumns);
    }
}