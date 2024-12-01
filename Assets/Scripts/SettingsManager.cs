using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;



//All project enums
public enum AlgoState
{
    unvisited, visited, traversed, unwalkable
}
public enum DataTypes
{
    AdjList, AdjMatrix, EdgeList
}
public enum AgentsType
{
    User, One, Five, Hundred, Thousand, TenThousand
}
public enum AlgoTypes
{
    BFS, DFS, AStar, Dijkstra, BellmanFord
}
public enum GraphTypes
{
    Random, EightGrid, FourGrid, Terrain1, Terrain2, Terrain3
}

public enum GraphicalTypes
{
    None, PathOnly, ShowUsed, ShowAll
}
public enum VisualizeTypes
{
    Instant, Fast, Medium, Slow, ReallySlow
}

//Graph abstract class
public abstract class Graph
{
    public abstract int GetSize();
    public abstract void ResetGraph();
    public abstract void ResetSameGraph();
    public abstract Dictionary<string, Node> GetNodes();


    //Get a random start and end point for NPCs
    public (Node start, Node end) GetRandomStartEnd()
    {
        List<string> keys = new List<string>(GetNodes().Keys);

        int firstIndex = UnityEngine.Random.Range(0, keys.Count);
        Node start = GetNodes()[keys[firstIndex]];

        Node end;
        do
        {
            int secondIndex = UnityEngine.Random.Range(0, keys.Count);
            end = GetNodes()[keys[secondIndex]];
        } while (start == end);

        return (start, end);
    }

    //Get random node on any graph. 
    //Can filter the walkable nodes
    public Node GetRandom(bool includeNotWalkable)
    {

        List<string> keys = new List<string>(GetNodes().Keys);

        int firstIndex = UnityEngine.Random.Range(0, keys.Count);

        Node node = GetNodes()[keys[firstIndex]];
        if(includeNotWalkable == false && node.GetIsWalkable() == false)
        {
            return GetRandom(includeNotWalkable);
        }
        else
        {
            return GetNodes()[keys[firstIndex]];
        }
    }

    //Update the graph when obstacles are moved
    public void UpdateWithObstacles()
    {
        foreach (var node in GetNodes())
        {
            node.Value.SetIsWalkable(true);
        }

        foreach (var obstacle in SettingsManager.Instance.obstacles)
        {
            foreach (var node in GetNodes())
            {
                if (obstacle.collider.bounds.Contains(node.Value.GetPosition()))
                {
                    node.Value.SetIsWalkable(false);
                }
            }
        }
    }
}


/// <summary>
/// Handles all the settings for the project
/// User can change them using the ui, these are changed by the gameManager script
/// </summary>

public class SettingsManager : MonoBehaviour
{
    //Singleton design pattern
    public static SettingsManager Instance { get; private set; }
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


    public Color lineDefaultColor;
    public Color lineVisitedColor;
    public Color lineTraversalColor;
    public Color lineNotWalkableColor;

    public Obstacle[] obstacles;
    public Terrain[] terrains;
    public Material nodeDefaultMaterial;
    public Material nodeVisitedMaterial;
    public Material nodeTraversalMaterial;
    public Material nodeNotWalkableMaterial;

    public Transform startTrans;
    public Transform endTrans;

    public int selectedTerrain = 0;

    public DataTypes dataTypes = DataTypes.EdgeList;
    public AlgoTypes algoTypes = AlgoTypes.BFS;
    public AgentsType agentTypes = AgentsType.User;
    public GraphTypes graphTypes = GraphTypes.Random;
    public GraphicalTypes graphicalType = GraphicalTypes.ShowAll;
    public int density = 10;
    public int minWeight = 0; 
    public int maxWeight = 10;
    private int size = 100;
    public int gridSize = 10;
    public bool visualize;
    public float maxLineSize = 3;
    public Vector3 obstacleSize;
    private VisualizeTypes visualizeType = VisualizeTypes.Instant;


    //Getters and Setters
    public Terrain GetTerrain()
    {
        if((int)graphTypes <= 2)
        {
            return null;
        }
        else
        {
            selectedTerrain = (int)graphTypes - 3;
        }
        return terrains[selectedTerrain];
    }
    public int GetSize()
    {
        return size;
    }
    public void SetSize(int type)
    {
        if (type == 0) size = 100;
        else if (type == 1) size = 2500;
        else if (type == 2) size = 10000;
        else if (type == 3) size = 25000;
        else if (type == 4) size = 100000;

        for (int i = 0; i < obstacles.Length; i++)
        {
            if ((int)graphTypes > 2) type = 3;
            obstacles[i].SetSize(type + 1);
            startTrans.localScale = obstacleSize * (type + 1);
            endTrans.localScale = obstacleSize * (type + 1);
        }

    }
    public float GetVisualizeSpeed()
    {
        if (visualizeType == VisualizeTypes.Fast) return 0.01f;
        else if (visualizeType == VisualizeTypes.Medium) return 0.1f;
        else if (visualizeType == VisualizeTypes.Slow) return 0.5f;
        else if (visualizeType == VisualizeTypes.ReallySlow) return 1;
        return 0;
    }
    public void SetVisualizeSpeed(int type)
    {
        visualizeType = (VisualizeTypes)type;
    }
    public int GetAgentCount()
    {
        if (agentTypes == AgentsType.One) return 1;
        else if (agentTypes == AgentsType.Five) return 5;
        else if (agentTypes == AgentsType.Hundred) return 100;
        else if (agentTypes == AgentsType.Thousand) return 1000;
        else if (agentTypes == AgentsType.TenThousand) return 2000;
        return 0;


    }
}



