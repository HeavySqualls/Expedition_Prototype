using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitView : MonoBehaviour
{
    Vector3 newPosition;

    Vector3 currentVelocity;
    float smoothTime = 0.5f;

    void Start()
    {
        newPosition = this.transform.position;
    }

  public void OnUnitMoved(Hex oldHex, Hex newHex)
    {
        // This GameObject is supposed to be a child of the hex we are standing in. 
        // This ensures that we are in the correct place in the hierachy. 
        // Our correct position when we arent moving is to be at 0,0 local position relative to our parent. 

        this.transform.position = oldHex.PositionFromCamera();
        newPosition = newHex.PositionFromCamera();
        currentVelocity = Vector3.zero;

        // TELEPORT INSTRUCTION
        if (Vector3.Distance(this.transform.position, newPosition) > 10) // (why do I need to be at 10 to move with lerp with this??)
        {
            // This OnUnitMoved is considerably more than the expected move between two adjacent tiles. 
            // Its probably a map seam thing, so just teleport. 

            this.transform.position = newPosition;

        }
        else
        {
            // TODO: We NEED a better way of signalling system and/or animation queueing...
            GameObject.FindObjectOfType<HexMap>().AnimationIsPlaying = true;
        }
    }

    void Update()
    {
        this.transform.position = Vector3.SmoothDamp(this.transform.position, newPosition, ref currentVelocity, smoothTime);

        //TODO: Figure out the best way to determine the end of our animation
        if(Vector3.Distance(this.transform.position, newPosition) < 0.1f)
            GameObject.FindObjectOfType<HexMap>().AnimationIsPlaying = false;

    }
}
