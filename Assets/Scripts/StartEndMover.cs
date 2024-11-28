using UnityEngine;

public class StartEndMover : MonoBehaviour
{
    private Transform selectedStartEnd; 
    public Color highlightColor = Color.yellow;
    private Color originalColor; 
    private Renderer obstacleRenderer;
    private bool canMove = true;
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canMove)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (selectedStartEnd == null)
                {
                    // Select an obstacle if clicked
                    if (hit.collider.CompareTag("StartEnd"))
                    {
                        SelectObstacle(hit.collider.transform);
                    }
                }
                else
                {
                    if (hit.collider.CompareTag("Terrain"))
                    {
                        MoveObstacle(hit.point);
                        DeselectObstacle();
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && selectedStartEnd != null)
        {
            DeselectObstacle();
        }
    }
    private void SelectObstacle(Transform obstacle)
    {
        selectedStartEnd = obstacle;
        obstacleRenderer = selectedStartEnd.GetComponent<Renderer>();

        if (obstacleRenderer != null)
        {
            originalColor = obstacleRenderer.material.color;
            obstacleRenderer.material.color = highlightColor;
        }
    }
    private void DeselectObstacle()
    {
        if (selectedStartEnd != null && obstacleRenderer != null)
        {
            obstacleRenderer.material.color = originalColor;
        }

        selectedStartEnd = null;
        obstacleRenderer = null;
    }

    private void MoveObstacle(Vector3 newPosition)
    {
        if (selectedStartEnd != null)
        {
            //newPosition.y = selectedStartEnd.position.y;
            selectedStartEnd.position = newPosition;

        }
    }
}