using UnityEngine;
/// <summary>
/// StartEndMover
/// Allows the user to click to select a start/end point and click again to place it in a new position
/// </summary>
public class StartEndMover : MonoBehaviour
{
    private Transform selectedStartEnd; 
    public Color highlightColor = Color.yellow;
    private Color originalColor; 
    private Renderer startEndRenderer;
    private bool canMove = true;
    public LayerMask terrainLayer;
    public LayerMask objectLayer;
    void Update()
    {
        //Check if user is clicking
        if (Input.GetMouseButtonDown(0) && canMove)
        {
            LayerMask selectedLayer = objectLayer;
            if (selectedStartEnd != null) selectedLayer = terrainLayer;
            //Send ray to see if hits a startEnd
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 10000, selectedLayer))
            {
                if (selectedStartEnd == null)
                {
                    // Select an start/end point if clicked
                    if (hit.collider.CompareTag("StartEnd"))
                    {
                        SelectStartEnd(hit.collider.transform);
                    }
                }
                else
                {
                    // If has selected a start/end point 
                    // Check if clicked on terrain to move it
                    if (hit.collider.CompareTag("Terrain"))
                    {
                        MoveStartEnd(hit.point);
                        DeselectStartEnd();
                    }
                }
            }
        }

        //Right click to deselect
        if (Input.GetMouseButtonDown(1) && selectedStartEnd != null)
        {
            DeselectStartEnd();
        }
    }
    //Select startEnd point
    //Change its material
    private void SelectStartEnd(Transform startEnd)
    {
        selectedStartEnd = startEnd;
        startEndRenderer = selectedStartEnd.GetComponent<Renderer>();

        if (startEndRenderer != null)
        {
            originalColor = startEndRenderer.material.color;
            startEndRenderer.material.color = highlightColor;
        }
    }
    //Deselect startEnd
    //Change its material
    private void DeselectStartEnd()
    {
        if (selectedStartEnd != null && startEndRenderer != null)
        {
            startEndRenderer.material.color = originalColor;
        }

        selectedStartEnd = null;
        startEndRenderer = null;
    }
    //Move startEnd
    //Tell Graph startEnd was move so it can update
    private void MoveStartEnd(Vector3 newPosition)
    {
        if (selectedStartEnd != null)
        {
            //newPosition.y = selectedStartEnd.position.y;
            selectedStartEnd.position = newPosition;

        }
    }
}