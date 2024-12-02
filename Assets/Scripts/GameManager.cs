using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main script for the game
/// Handles the ui, npc, graph...
/// </summary>
public class GameManager : MonoBehaviour
{
    public Coroutine currentCoroutine;
    public Graph graph;

    //UI dropdowns
    public TMP_Dropdown algoDropdown;
    public TMP_Dropdown dataDropdown;
    public TMP_Dropdown sizeDropdown;
    public TMP_Dropdown graphDropdown;
    public TMP_Dropdown agentsDropdown;
    public TMP_Dropdown graphicalDropdown;
    public TMP_Dropdown visualizeDropdown;
    //Start and end node
    private Node startNode;
    private Node endNode;



    private bool graphHasChanged = false;
    private bool graphicalHasChanged = false;
    private int timesRan;
    public bool isRunning;

    //Event handle
    public delegate void EventHandler();
    public event EventHandler graphObstaclesUpdatedEvent;
    public event EventHandler graphUpdatedEvent;
    float npcAgentTimer = 1000;
    public Queue<NPCAgent> agents = new Queue<NPCAgent>();


    // Singleton 
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
    //Gets closest node to a position
    public Node GetClosestNode(Vector3 pos)
    {
        return Utilities.GetClosestNode(graph, pos);
    }
    //Gets random node on a graph
    public Node GetRandomNodeOnGraph(bool includeNotWalkable)
    {
        if(graph == null) return null;
        return graph.GetRandom(includeNotWalkable);
    }

    private RunReport currentReport;
    public void GetNewReport()
    {
        currentReport = new RunReport(
            SettingsManager.Instance.dataTypes,
            SettingsManager.Instance.algoTypes,
            SettingsManager.Instance.graphTypes,
            SettingsManager.Instance.density,
            SettingsManager.Instance.minWeight,
            SettingsManager.Instance.maxWeight,
            SettingsManager.Instance.GetSize(),
            SettingsManager.Instance.gridSize,
            SettingsManager.Instance.maxLineSize
        );
    }

    //Request path
    //Returns true or false if was successful or is busy
    public bool GetPath(Node start, Node end, Action<List<Node>> callback)
    {



        if (graph == null || isRunning == true) return false;
        if (SettingsManager.Instance.algoTypes == AlgoTypes.BFS)
        {
            StartCoroutine(BreadthFirstSearch.BFS_OnGraph(currentReport, graph, start, end, callback));
        }
        else if (SettingsManager.Instance.algoTypes == AlgoTypes.DFS)
        {
            StartCoroutine(DepthFirstSearch.DFS_OnGraph(currentReport, graph, start, end, callback));
        }
        else if (SettingsManager.Instance.algoTypes == AlgoTypes.Dijkstra)
        {
            StartCoroutine(DijkstraAlgorithm.Dijkstra_OnGraph(currentReport, graph, start, end, callback));
        }
        else if (SettingsManager.Instance.algoTypes == AlgoTypes.AStar)
        {
            StartCoroutine(AStarAlgorithm.AStar_OnGraph(currentReport, graph, start, end, callback));
        }
        else if (SettingsManager.Instance.algoTypes == AlgoTypes.BellmanFord)
        {
            StartCoroutine(BellfordAlgorithm.Bellford_OnGraph(currentReport, graph, start, end, callback));
        }
        return true;
    }

    public void MeasureGraph()
    {

    }

    //Runs the settings on the graph
    //Called by the ui button
    public void RunGraph()
    {
        
        if (isRunning)
        {
            isRunning = false;
            StopAllCoroutines();
            graph.ResetGraph();
        }
        isRunning = true;
        timesRan += 1;
        graphHasChanged = false;
        graphicalHasChanged = false;
        npcAgentTimer = .5f;
        SettingsManager.Instance.algoTypes = (AlgoTypes)algoDropdown.value;

        if (SettingsManager.Instance.dataTypes != (DataTypes)dataDropdown.value) graphHasChanged = true;
        SettingsManager.Instance.dataTypes = (DataTypes)dataDropdown.value;



        int prevSize = SettingsManager.Instance.GetSize();
        SettingsManager.Instance.SetSize(sizeDropdown.value);
        if (prevSize != SettingsManager.Instance.GetSize()) graphHasChanged = true;

        if (SettingsManager.Instance.graphTypes != (GraphTypes)graphDropdown.value) graphHasChanged = true;
        SettingsManager.Instance.graphTypes = (GraphTypes)graphDropdown.value;


        SettingsManager.Instance.agentTypes = (AgentsType)agentsDropdown.value;

        graphHasChanged = true;/////


        //Disable visualize if going to slow system
        if ((int)graphicalDropdown.value > 1 && sizeDropdown.value > 1)
        {
            graphicalDropdown.value = 1;
            graphicalHasChanged = true;
            SettingsManager.Instance.graphicalType = GraphicalTypes.PathOnly;
        }
        else if((int)SettingsManager.Instance.agentTypes > 1)
        {
            graphicalDropdown.value = 0;
            graphicalHasChanged = true;
            SettingsManager.Instance.graphicalType = GraphicalTypes.None;
        }
        else
        {
            if (SettingsManager.Instance.graphicalType != (GraphicalTypes)graphicalDropdown.value) graphicalHasChanged = true;
            SettingsManager.Instance.graphicalType = (GraphicalTypes)graphicalDropdown.value;
        }


        SettingsManager.Instance.SetVisualizeSpeed(visualizeDropdown.value);
        if (visualizeDropdown.value == 0) SettingsManager.Instance.visualize = false;
        else SettingsManager.Instance.visualize = true;


        if (currentCoroutine != null) StopCoroutine(currentCoroutine);


        if (graphHasChanged || timesRan == 1)
        {
            GetNewReport();

            for (int i = 0; i < SettingsManager.Instance.terrains.Length; i++)
            {
                SettingsManager.Instance.terrains[i].gameObject.SetActive(false);
            }
            if ((int)SettingsManager.Instance.graphTypes > 2)
            {
                SettingsManager.Instance.GetTerrain().gameObject.SetActive(true);
            }
    


            if(graph != null) graph.ResetGraph();
            //Debug.Log(graphHasChanged);
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


        graphUpdatedEvent?.Invoke();
        if (SettingsManager.Instance.agentTypes != AgentsType.User)
        {
            isRunning = false;
            SettingsManager.Instance.startTrans.gameObject.SetActive(false);
            SettingsManager.Instance.endTrans.gameObject.SetActive(false);
            return;
        }
        SettingsManager.Instance.startTrans.gameObject.SetActive(true);
        SettingsManager.Instance.endTrans.gameObject.SetActive(true);

        if (SettingsManager.Instance.algoTypes == AlgoTypes.BFS)
        {
            currentCoroutine = StartCoroutine(BreadthFirstSearch.BFS_OnGraph(currentReport, graph, startNode, endNode, (result) =>
            {
                isRunning = false;
            }));


        }
        else if (SettingsManager.Instance.algoTypes == AlgoTypes.DFS)
        {
            currentCoroutine = StartCoroutine(DepthFirstSearch.DFS_OnGraph(currentReport, graph, startNode, endNode, (result) =>
            {
                isRunning = false;

            }));
        }
        else if (SettingsManager.Instance.algoTypes == AlgoTypes.Dijkstra)
        {
            currentCoroutine = StartCoroutine(DijkstraAlgorithm.Dijkstra_OnGraph(currentReport, graph, startNode, endNode, (result) =>
            {
                isRunning = false;

            }));
        }
        else if (SettingsManager.Instance.algoTypes == AlgoTypes.AStar)
        {
            currentCoroutine = StartCoroutine(AStarAlgorithm.AStar_OnGraph(currentReport, graph, startNode, endNode, (result) =>
            {
                isRunning = false;
            }));
        }
        else if (SettingsManager.Instance.algoTypes == AlgoTypes.BellmanFord)
        {
            currentCoroutine = StartCoroutine(BellfordAlgorithm.Bellford_OnGraph(currentReport, graph, startNode, endNode, (result) =>
            {
                isRunning = false;
            }));
        }

    }



    public void ObstacleWasMoved()
    {
        //graph.UpdateWithObstacles();

        //graphObstaclesUpdatedEvent?.Invoke();

    }

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


    public void OpenReport()
    {

    }
}

