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

    public float maxX;
    public float maxZ;
    public float minX;
    public float minZ;

    public float minAngle;
    public float maxAngle; 

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
            Vector3 newLocalPosition = mainCamera.transform.position + mainCamera.transform.forward * zoomAmount;


            mainCamera.transform.position = newLocalPosition;
        }
    }

    private void HandleRotation()
    {
        // Right mouse button for rotation
        if (Input.GetMouseButton(1))
        {
            float horizontal = Input.GetAxis("Mouse X");
            float vertical = Input.GetAxis("Mouse Y");

            //Rotate the parent object
            cameraParent.Rotate(Vector3.up, horizontal * rotationSpeed * Time.deltaTime, Space.World);
            cameraParent.Rotate(Vector3.right, -vertical * rotationSpeed * Time.deltaTime, Space.Self);

            if (cameraParent.localEulerAngles.x > maxAngle)
                cameraParent.localEulerAngles = new Vector3(maxAngle, cameraParent.localEulerAngles.y, 0);

            else if (cameraParent.localEulerAngles.x < minAngle)
                cameraParent.localEulerAngles = new Vector3(minAngle, cameraParent.localEulerAngles.y, 0);

        }
    }

    private void HandleDrag()
    {
        // Left mouse button to start drag
        if (Input.GetMouseButtonDown(0)) 
        {
            isDragging = true;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 10000, layerTarget))
            {
                dragStartPos = hit.point;
            }
        }
        // Left mouse button up to stop drag
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

                Vector3 newPos = cameraParent.position;
                newPos += dragDelta * moveSpeed * Time.deltaTime;

                //Limits for drag
                if(newPos.x < minX)
                {
                    newPos = new Vector3(minX, 0, newPos.z);
                }
                if (newPos.x > maxX)
                {
                    newPos = new Vector3(maxX, 0, newPos.z);
                }
                if (newPos.z < minZ)
                {
                    newPos = new Vector3(newPos.x, 0, minZ);
                }
                if (newPos.z > maxZ)
                {
                    newPos = new Vector3(newPos.x, 0, maxZ);
                }
                newPos = new Vector3(newPos.x, 0, newPos.z);
                cameraParent.position = newPos;
            }
        }
    }
}