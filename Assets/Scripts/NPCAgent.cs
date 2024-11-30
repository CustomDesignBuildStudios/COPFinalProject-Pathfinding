using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class NPCAgent : MonoBehaviour
{
    //Buffer distance to get new node 
    public float waypointTolerance = 1;

    private Node currentNode;
    private Node endNode;
    private List<Node> path;
    private int pathIndex = 0;
    public bool isGettingPath = false;
    public float moveSpeed = 2;
    public float requestNewPathTime = 2;
    private float timeToGetNewPath = 2;

    //Init
    //Subscribes to graph updated event
    void Start()
    {
        GameManager.Instance.graphObstaclesUpdatedEvent += RequestNewPath;
        timeToGetNewPath = requestNewPathTime;
    }


    public void EnableAgent()
    {
        gameObject.SetActive(true);
    }
    //Sends agent back to pool
    public void DisableAgent()
    {
        isGettingPath = false;
        currentNode = null;
        endNode = null;
        pathIndex = 0;
        path = null;
        gameObject.SetActive(false);
    }

    
    void FixedUpdate()
    {
        //Trys to get new path by timer
        timeToGetNewPath -= Time.deltaTime;
        if (path == null && isGettingPath == false && timeToGetNewPath < 0)
        {
            timeToGetNewPath = requestNewPathTime + Random.Range(0,2);
            RequestNewPath();
        }
        //If has path
        else if(path != null)
        {
            //moves to next point if point is far away
            if (Vector3.Distance(this.transform.position,currentNode.GetPosition()) > waypointTolerance)
            {
                transform.position = Vector3.MoveTowards(transform.position, currentNode.GetPosition(), moveSpeed * Time.deltaTime);

            }
            //if close to next point get next point or has finished path
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
    //Trys to get new path
    void RequestNewPath()
    {
        path = null;
        if (this.gameObject.activeSelf)
        {
            isGettingPath = true;
            if (currentNode == null)
            {
                currentNode = GameManager.Instance.GetClosestNode(this.transform.position);
                endNode = GameManager.Instance.GetRandomNodeOnGraph(false);
            }
            bool isGetting = GameManager.Instance.GetPath(currentNode, endNode, (result) =>
            {
                pathIndex = 0;
                path = result;
                isGettingPath = false;
                currentNode = result[pathIndex];
            });
            if (isGetting == false) isGettingPath = false;
        }
    }
}