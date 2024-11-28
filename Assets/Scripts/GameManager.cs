using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public bool GetPath(Node start, Node end, Action<List<Node>> callback)
    {
        if(graph == null || isRunning == true) return false;
        if (SettingsManager.Instance.algoTypes == AlgoTypes.BFS)
        {
            StartCoroutine(BreadthFirstSearch.BFS_OnGraph(graph, start, end, callback));
        }
        else if (SettingsManager.Instance.algoTypes == AlgoTypes.DFS)
        {
            StartCoroutine(DepthFirstSearch.DFS_OnGraph(graph, start, end, callback));
        }
        else if (SettingsManager.Instance.algoTypes == AlgoTypes.Dijkstra)
        {
            StartCoroutine(DijkstraAlgorithm.Dijkstra_OnGraph(graph, start, end, callback));
        }
        else if (SettingsManager.Instance.algoTypes == AlgoTypes.AStar)
        {
            StartCoroutine(AStarAlgorithm.AStar_OnGraph(graph, start, end, callback));
        }
        return true;
    }
    public Node GetClosestNode(Vector3 pos)
    {
        return Utilities.GetClosestNode(graph, pos);
    }
    public Node GetRandomNodeOnGraph(bool includeNotWalkable)
    {
        if(graph == null) return null;
        return graph.GetRandom(includeNotWalkable);
    }



    public Coroutine currentCoroutine;
    public Graph graph;

    public TMP_Dropdown algoDropdown;
    public TMP_Dropdown dataDropdown;
    public TMP_Dropdown sizeDropdown;
    public TMP_Dropdown graphDropdown;
    public TMP_Dropdown agentsDropdown;
    public TMP_Dropdown graphicalDropdown;
    public TMP_Dropdown visualizeDropdown;
    private Node startNode;
    private Node endNode;
    private bool graphHasChanged = false;
    private bool graphicalHasChanged = false;
    private int timesRan;
    public bool isRunning;

    public GameObject startPoint;
    public GameObject endPoint;

    public Queue<NPCAgent> agents = new Queue<NPCAgent>();
    public void RunGraph()
    {
        if (isRunning) return;
        isRunning = true;
        timesRan += 1;
        graphHasChanged = false;
        graphicalHasChanged = false;
        npcAgentTimer = .5f;
        SettingsManager.Instance.algoTypes = (AlgoTypes)algoDropdown.value;

        if (SettingsManager.Instance.dataTypes != (DataTypes)dataDropdown.value) graphHasChanged = true;
        SettingsManager.Instance.dataTypes = (DataTypes)dataDropdown.value;

        SettingsManager.Instance.graphicalType = (GraphicalTypes)graphicalDropdown.value;
        Debug.Log((int)SettingsManager.Instance.graphicalType);

        int prevSize = SettingsManager.Instance.GetSize();
        SettingsManager.Instance.SetSize(sizeDropdown.value);
        if (prevSize != SettingsManager.Instance.GetSize()) graphHasChanged = true;

        if (SettingsManager.Instance.graphTypes != (GraphTypes)graphDropdown.value) graphHasChanged = true;
        SettingsManager.Instance.graphTypes = (GraphTypes)graphDropdown.value;


        SettingsManager.Instance.agentTypes = (AgentsType)agentsDropdown.value;



        if (SettingsManager.Instance.graphicalType != (GraphicalTypes)graphicalDropdown.value) graphicalHasChanged = true;
        SettingsManager.Instance.graphicalType = (GraphicalTypes)graphicalDropdown.value;

        SettingsManager.Instance.SetVisualizeSpeed(visualizeDropdown.value);
        if (visualizeDropdown.value == 0) SettingsManager.Instance.visualize = false;
        else SettingsManager.Instance.visualize = true;


        if (currentCoroutine != null) StopCoroutine(currentCoroutine);



        if (graphHasChanged || timesRan == 1)
        {
            if(graph != null) graph.ResetGraph();
            Debug.Log(graphHasChanged);
            if (SettingsManager.Instance.dataTypes == DataTypes.AdjList)
            {
                graph = AdjListCreator.CreateGraph();
            }
            else if (SettingsManager.Instance.dataTypes == DataTypes.EdgeList)
            {
                graph = EdgeListCreator.CreateGraph();
            }
        }
        else
        {
            graph.ResetSameGraph();
        }
        graph.UpdateWithObstacles();

        startNode = Utilities.GetClosestNode(graph, SettingsManager.Instance.startTrans.position);
        endNode = Utilities.GetClosestNode(graph, SettingsManager.Instance.endTrans.position);

        List<Node> path = new List<Node>();

        float startTime = Time.realtimeSinceStartup;


        if (SettingsManager.Instance.agentTypes != AgentsType.User)
        {
            isRunning = false;
            return;
        }

        if (SettingsManager.Instance.algoTypes == AlgoTypes.BFS)
        {
            currentCoroutine = StartCoroutine(BreadthFirstSearch.BFS_OnGraph(graph, startNode, endNode, (result) =>
            {
                float endTime = Time.realtimeSinceStartup;
                //dataPoints.Add(report);
                isRunning = false;

            }));


        }
        else if (SettingsManager.Instance.algoTypes == AlgoTypes.DFS)
        {
            currentCoroutine = StartCoroutine(DepthFirstSearch.DFS_OnGraph(graph, startNode, endNode, (result) =>
            {
                float endTime = Time.realtimeSinceStartup;
                //dataPoints.Add(report);
                isRunning = false;

            }));
        }
        else if (SettingsManager.Instance.algoTypes == AlgoTypes.Dijkstra)
        {
            currentCoroutine = StartCoroutine(DijkstraAlgorithm.Dijkstra_OnGraph(graph, startNode, endNode, (result) =>
            {
                float endTime = Time.realtimeSinceStartup;
                //dataPoints.Add(report);
                isRunning = false;

            }));
        }
        else if (SettingsManager.Instance.algoTypes == AlgoTypes.AStar)
        {
            currentCoroutine = StartCoroutine(AStarAlgorithm.AStar_OnGraph(graph, startNode, endNode, (result) =>
            {
                float endTime = Time.realtimeSinceStartup;
                //dataPoints.Add(report);
                isRunning = false;
            }));
        }
    }

    public delegate void EventHandler();
    public event EventHandler graphObstaclesUpdatedEvent;

    public void ObstacleWasMoved()
    {
        graph.UpdateWithObstacles();

        graphObstaclesUpdatedEvent?.Invoke();

    }

    float npcAgentTimer = 1000;
    private void Update()
    {
        npcAgentTimer -= Time.deltaTime;
        if(npcAgentTimer < 0)
        {
            npcAgentTimer = 0.01f;
            int desiredAgentCount = SettingsManager.Instance.GetAgentCount();

            if (agents.Count > desiredAgentCount)
            {
                NPCAgent agent = agents.Dequeue();
                agent.DisableAgent();
            }
            else if(agents.Count < desiredAgentCount)
            {
                agents.Enqueue(PoolManager.Instance.GetAgent());
            }
        }
    }










    //public List<DataPoint> dataPoints;

    public void OpenReport()
    {

    }
}

public struct DataPoint
{
    public DataTypes dataTypes;
    public AlgoTypes algoTypes;
    public GraphTypes graphTypes;
    public int density;
    public int minWeight;
    public int maxWeight;
    private int size;
    public int gridSize;
    public float maxLineSize;
    public float timeToRun;

}
