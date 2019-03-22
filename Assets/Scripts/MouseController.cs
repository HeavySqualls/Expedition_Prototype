using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    // Generic bookkeeping variables 
    HexMap hexMap;
    Hex hexUnderMouse;
    Hex hexLastUnderMouse;
    Vector3 LastMousePosition; // from Input.mousePosition

    // Camera Dragging bookkeeping variables
    int mouseDragThreshhold = 1; // Threshold of mouse movement to start dragging
    Vector3 lastMouseGroundPlanePosition;
    Vector3 cameraTargetOffset;

    // Unit movement
    Unit __selectedUnit = null; // dont use this directly 
    public Unit SelectedUnit
    {
        get { return __selectedUnit; }
        set {
            __selectedUnit = value;            
            UnitSelectionPanel.SetActive(__selectedUnit != null); // if selected unit is NOT null, UnitSelectionPanel is set to active
            }
    }

    LineRenderer lineRenderer;
    Hex[] hexPath;

    //UI Controls
    public GameObject UnitSelectionPanel;

    delegate void UpdateFunc();
    UpdateFunc Update_CurrentFunc;

    public LayerMask LayerID_HexTiles;


    void Start()
    {
        Update_CurrentFunc = Update_DetectModeStart;

        hexMap = GameObject.FindObjectOfType<HexMap>();

        lineRenderer = transform.GetComponentInChildren<LineRenderer>();

        
    }


    void Update()
    {
        hexUnderMouse = MouseToHex(); 

        if (Input.GetKeyDown(KeyCode.Q))
        {
            SelectedUnit = null;
            CancelUpdateFunc();
        }

        Update_CurrentFunc();

        Update_ScrollZoom();

        LastMousePosition = Input.mousePosition;

        hexLastUnderMouse = hexUnderMouse;

        if (SelectedUnit != null)
        {
            DrawPath((hexPath != null) ? hexPath : SelectedUnit.GetHexPath());
        }
        else
        {
            DrawPath(null); // clear the path display
        }
    }


    void DrawPath(Hex[] hexPath)
    {
        if (hexPath == null || hexPath.Length == 0)
        {
            lineRenderer.enabled = false;
            return;
        }
        lineRenderer.enabled = true;

        Vector3[] positions = new Vector3[hexPath.Length];

        for (int i = 0; i < hexPath.Length; i++)
        {
            GameObject hexGO = hexMap.GetHexGO(hexPath[i]);
            positions[i] = hexGO.transform.position + (Vector3.up * 0.1f);
        }

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }


    void CancelUpdateFunc()
    {
        Update_CurrentFunc = Update_DetectModeStart;

        // Also do cleanup with any UI stuff associated with modes. 

        hexPath = null;
    }

    // ****** DEFAULT MOUSE MODE ******* //

    void Update_DetectModeStart()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Button DOWN");
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Mouse button UP.... CLICK!");
            // TODO: Are we clicking on a hex with a unit? if so, select it.  

            Unit[] us = hexUnderMouse.Units();

            // TODO: Implement cycling through multiple units if more than one is on a tile

            if (us.Length > 0 )
            {
                SelectedUnit = us[0];
                // Update_CurrentFunc = Update_UnitMovement;

                // NOTE: Selecting a unit does NOT change mouse mode.
            }
        }
        else if (SelectedUnit != null && Input.GetMouseButtonDown(1))
        {
            Update_CurrentFunc = Update_UnitMovement;
        }
        else if (Input.GetMouseButton(0) && Vector3.Distance(Input.mousePosition, LastMousePosition) > mouseDragThreshhold)
        {
            Update_CurrentFunc = Update_CameraDrag;
            lastMouseGroundPlanePosition = MouseToGroundPlane(Input.mousePosition);
            Update_CurrentFunc();
        }
        else if (SelectedUnit != null && Input.GetMouseButton(1))
        {
            // we have a selected unit and we are holding down the mouse button. 
            // We are in unit movement mode, show a path from unit to mouse position via the pathfinding system. 
        }
    }
   
    Hex MouseToHex()  // Take the mouse position, turn it into a ray, then use physics.raycast to return hit info
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        int layerMask = LayerID_HexTiles.value;

        if (Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity, layerMask))
        {
            //Debug.Log(hitInfo.collider.name); // the collider is a child of the "correct" gameObject that we want.

            GameObject hexGO = hitInfo.rigidbody.gameObject;

            return hexMap.GetHexFromGO(hexGO);
        }
        Debug.Log("Found nothing");
        return null;
    }

    Vector3 MouseToGroundPlane(Vector3 mousePos)
    {
        Ray mouseRay = Camera.main.ScreenPointToRay (mousePos);

        if (mouseRay.direction.y >= 0)
        {
            return Vector3.zero;
        }
        float rayLength = (mouseRay.origin.y / mouseRay.direction.y);
        return mouseRay.origin - (mouseRay.direction * rayLength);
    }


    void Update_UnitMovement()
    {
        if (Input.GetMouseButtonUp(1) || SelectedUnit == null)
        {
            Debug.Log("Complete unit movement.");

            if (SelectedUnit != null)
            {
                SelectedUnit.SetHexPath(hexPath);
            }

            CancelUpdateFunc();
            return;
        }

        // We have selected unit
        // Look at the hex under the mouse
        // is this a different hex than before (or we dont already have a path)
        if (hexPath == null || hexUnderMouse != hexLastUnderMouse)
        {
            // Do a pathfinding search to that hex
            hexPath = QPath.QPath.FindPath<Hex>(hexMap, SelectedUnit, SelectedUnit.Hex, hexUnderMouse, Hex.CostEstimate);
        }
    } 

    void Update_CameraDrag()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Cancelling camera drag.");
            CancelUpdateFunc();
            return;
        }

        // Sends out the ray on the Y axis and returns with a value unless Y is 0
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (mouseRay.direction.y >= 0)
        {
            return;
        }
        Vector3 hitPos = MouseToGroundPlane(Input.mousePosition);

        Vector3 diff = lastMouseGroundPlanePosition - hitPos;
        Camera.main.transform.Translate(diff, Space.World);

        lastMouseGroundPlanePosition = hitPos = MouseToGroundPlane(Input.mousePosition);
    }  


    void Update_ScrollZoom()
        {
            float scrollAmmount = Input.GetAxis("Mouse ScrollWheel");
            float minHeight = 2f; // <------------------------------------------------------------ADJUSTMENT FOR CLOSE ZOOM
            float maxHeight = 30f; // <-----------------------------------------------------------ADJUSTMENT FOR FAR ZOOM
            // Move camera towards hitPos
            Vector3 hitPos = MouseToGroundPlane(Input.mousePosition);
            Vector3 dir = hitPos - Camera.main.transform.position;

            Vector3 p = Camera.main.transform.position;
 
            // TODO: Maybe you should still slide around at 20 zoom??

            // Stop zooming out at a certain distance
            if (scrollAmmount > 0 || p.y < (maxHeight - 0.1f))
            {
                cameraTargetOffset += dir * scrollAmmount;
            }
            Vector3 lastCameraPosition = Camera.main.transform.position;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, Camera.main.transform.position + cameraTargetOffset, Time.deltaTime * 5f);
            cameraTargetOffset -= Camera.main.transform.position - lastCameraPosition;

            p = Camera.main.transform.position;
            if (p.y < minHeight)// Minimum zoom-in distance
            {
                p.y = minHeight;
            }
            if (p.y > maxHeight) // Maximum zoom-in distance
            {
                p.y = maxHeight;
            }
            Camera.main.transform.position = p;

        Camera.main.transform.rotation = Quaternion.Euler(
        Mathf.Lerp(25, 80, Camera.main.transform.position.y / maxHeight),
        Camera.main.transform.rotation.eulerAngles.y,
        Camera.main.transform.rotation.eulerAngles.z
        );
    }
}
