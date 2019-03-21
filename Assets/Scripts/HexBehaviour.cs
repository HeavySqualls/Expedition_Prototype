using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexBehaviour : MonoBehaviour
{
    public Hex Hex;
    public HexMap NEWHexTileMap;
    
    public void UpdatePosition()
    {
        this.transform.position = Hex.PositionFromCamera(Camera.main.transform.position, NEWHexTileMap.numRows, NEWHexTileMap.numColumns);
    }
}
