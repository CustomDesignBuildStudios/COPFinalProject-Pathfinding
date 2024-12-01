using UnityEngine;

/// <summary>
/// Obstacle Mover
/// Allows the user to click to select a obstacle and click again to place it in a new position
/// </summary>
public class ObstacleMover : MonoBehaviour
{
    private Transform selectedObstacle; 
    public Color highlightColor = Color.yellow;
    private Color originalColor; 
    private Renderer obstacleRenderer;
    private bool canMoveObstacles = true;
    public LayerMask terrainLayer;
    public LayerMask objectLayer;
    void Update()
    {
        //Check if user is clicking
        if (Input.GetMouseButtonDown(0) && canMoveObstacles)
        {
            LayerMask selectedLayer = objectLayer;
            if (selectedObstacle != null) selectedLayer = terrainLayer;
            //Send ray to see if hits a obstacle
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 10000, selectedLayer))
            {
                if (selectedObstacle == null)
                {
                    // Select an obstacle if clicked
                    if (hit.collider.CompareTag("Obstacle"))
                    {
                        SelectObstacle(hit.collider.transform);
                    }
                }
                else
                {
                    // If has selected an obstacle 
                    // Check if clicked on terrain to move it
                    if (hit.collider.CompareTag("Terrain"))
                    {
                        MoveObstacle(hit.point);
                        DeselectObstacle();
                    }
                    else
                    {
                        DeselectObstacle();

                    }
                }
            }
        }
        //Right click to deselect
        if (Input.GetMouseButtonDown(1) && selectedObstacle != null)
        {
            DeselectObstacle();
        }
    }
    //Select obstacle
    //Change its material
    private void SelectObstacle(Transform obstacle)
    {
        selectedObstacle = obstacle;
        obstacleRenderer = selectedObstacle.GetComponent<Renderer>();
        Obstacle ob = selectedObstacle.GetComponent<Obstacle>();
        ob.isSelected = true;

        if (obstacleRenderer != null)
        {
            originalColor = obstacleRenderer.material.color;
            obstacleRenderer.material.color = highlightColor;
        }
    }
    //Deselect obstacle
    //Change its material
    private void DeselectObstacle()
    {
        if (selectedObstacle != null && obstacleRenderer != null)
        {
            obstacleRenderer.material.color = originalColor;
        }

        selectedObstacle = null;
        obstacleRenderer = null;
    }
    //Move obstacle
    //Tell Graph obstacle was move so it can update
    private void MoveObstacle(Vector3 newPosition)
    {
        if (selectedObstacle != null)
        {
            Obstacle ob = selectedObstacle.GetComponent<Obstacle>();
            ob.isSelected = false;

            selectedObstacle.position = newPosition;
            GameManager.Instance.ObstacleWasMoved();
        }
    }
}