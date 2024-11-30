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

    void Update()
    {
        //Check if user is clicking
        if (Input.GetMouseButtonDown(0) && canMoveObstacles)
        {
            //Send ray to see if hits a obstacle
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
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
            selectedObstacle.position = newPosition;
            GameManager.Instance.ObstacleWasMoved();
        }
    }
}