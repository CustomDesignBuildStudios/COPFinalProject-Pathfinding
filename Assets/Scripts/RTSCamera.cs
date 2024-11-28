using UnityEngine;

public class RTSCamera : MonoBehaviour
{
    public Transform cameraParent; // Parent object of the camera for orbiting
    public Camera mainCamera; // The camera itself
    public float zoomSpeed = 10f; // Speed of zooming
    public float moveSpeed = 10f; // Speed of dragging movement
    public float rotationSpeed = 100f; // Speed of rotation
    public float minZoomDistance = 5f; // Minimum zoom distance
    public float maxZoomDistance = 50f; // Maximum zoom distance

    private Vector3 dragStartPos;
    private bool isDragging = false;

    private void Update()
    {
        HandleZoom();
        HandleRotation();
        HandleDrag();
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            // Move the camera along its local Z-axis
            float zoomAmount = scroll * zoomSpeed * Time.deltaTime;
            Vector3 newLocalPosition = mainCamera.transform.localPosition + Vector3.forward * zoomAmount;

            // Clamp the camera's distance from the parent object
            float distance = newLocalPosition.magnitude;
            if (distance >= minZoomDistance && distance <= maxZoomDistance)
            {
                mainCamera.transform.localPosition = newLocalPosition;
            }
        }
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButton(1)) // Right mouse button for rotation
        {
            float horizontal = Input.GetAxis("Mouse X");
            float vertical = Input.GetAxis("Mouse Y");

            // Rotate the parent object
            cameraParent.Rotate(Vector3.up, horizontal * rotationSpeed * Time.deltaTime, Space.World);
            cameraParent.Rotate(Vector3.right, -vertical * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    private void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button to start drag
        {
            isDragging = true;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                dragStartPos = hit.point;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 dragEndPos = hit.point;
                Vector3 dragDelta = dragStartPos - dragEndPos;

                cameraParent.position += dragDelta * moveSpeed * Time.deltaTime;
            }
        }
    }
}