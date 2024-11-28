using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class NPCAgent : MonoBehaviour
{
    public float waypointTolerance = 1.0f; // Distance to consider the waypoint reached



    private Node currentNode;
    private Node endNode;
    private List<Node> path;
    private int pathIndex = 0;
    public bool isGettingPath = false;

    private bool visualizePath = false;
    public void VisualizePath()
    {
        visualizePath = true;
    }
    public void HidePath()
    {
        visualizePath = false;
    }

    public void EnableAgent()
    {
        gameObject.SetActive(true);
    }
    public void DisableAgent()
    {
        visualizePath = false;
        isGettingPath = false;
        currentNode = null;
        endNode = null;
        pathIndex = 0;
        path = null;
        gameObject.SetActive(false);
    }


    void Start()
    {
        GameManager.Instance.graphObstaclesUpdatedEvent += RequestNewPath;
    }




    public float moveSpeed = 2;
    public float requestNewPathTime = 2;
    public float timeToGetNewPath = 2;
    void FixedUpdate()
    {
        timeToGetNewPath -= Time.deltaTime;
        if (path == null && isGettingPath == false && timeToGetNewPath < 0)
        {
            timeToGetNewPath = requestNewPathTime;
            RequestNewPath();
        }
        else if(path != null)
        {
            if (Vector3.Distance(this.transform.position,currentNode.GetPosition()) > waypointTolerance)
            {
                transform.position = Vector3.MoveTowards(transform.position, currentNode.GetPosition(), moveSpeed * Time.deltaTime);

            }
            else
            {
                if(currentNode == endNode) {
                    
                    path = null;
                    currentNode = null;
                    endNode = null;
                }
                else
                {
                    pathIndex++;
                    currentNode = path[pathIndex];
                }

            }
        }
    }

    void RequestNewPath()
    {
        path = null;


        Debug.Log("RequestNewPath");
        isGettingPath = true;
        if (currentNode == null)
        {
            currentNode = GameManager.Instance.GetClosestNode(this.transform.position);
            endNode = GameManager.Instance.GetRandomNodeOnGraph(false);
        }


        bool isGetting = GameManager.Instance.GetPath(currentNode, endNode, (result) =>
        {
            Debug.Log("DONE");
            pathIndex = 0;
            path = result;
            isGettingPath = false;
            currentNode = result[pathIndex];
        });
        if (isGetting == false) isGettingPath = false;
    }
}