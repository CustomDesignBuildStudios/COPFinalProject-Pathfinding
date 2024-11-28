using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    private Transform selectedObstacle; 
    public Color highlightColor = Color.yellow;
    private Color originalColor; 
    private Renderer obstacleRenderer;
    private bool canMoveObstacles = true;
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canMoveObstacles)
        {
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
                    if (hit.collider.CompareTag("Terrain"))
                    {
                        MoveObstacle(hit.point);
                        DeselectObstacle();
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && selectedObstacle != null)
        {
            DeselectObstacle();
        }
    }
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
    private void DeselectObstacle()
    {
        if (selectedObstacle != null && obstacleRenderer != null)
        {
            obstacleRenderer.material.color = originalColor;
        }

        selectedObstacle = null;
        obstacleRenderer = null;
    }

    private void MoveObstacle(Vector3 newPosition)
    {
        if (selectedObstacle != null)
        {
            //newPosition.y = selectedObstacle.position.y;
            selectedObstacle.position = newPosition;

            GameManager.Instance.ObstacleWasMoved();
        }
    }
}