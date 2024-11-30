using UnityEngine;

public class RTSCamera : MonoBehaviour
{
    public Transform cameraParent;
    public Camera mainCamera; 
    public float zoomSpeed = 10f; 
    public float moveSpeed = 10f; 
    public float rotationSpeed = 100f; 
    public float minZoomDistance = 5f; 
    public float maxZoomDistance = 50f;
    public LayerMask layerTarget;
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
            Vector3 newLocalPosition = mainCamera.transform.localPosition + mainCamera.transform.forward * zoomAmount;

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
            if (Physics.Raycast(ray, out RaycastHit hit, 10000, layerTarget))
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
            if (Physics.Raycast(ray, out RaycastHit hit,10000, layerTarget))
            {
                Vector3 dragEndPos = hit.point;
                Vector3 dragDelta = dragStartPos - dragEndPos;

                cameraParent.position += dragDelta * moveSpeed * Time.deltaTime;
            }
        }
    }
}