using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// PoolManager manages a pool for nodes, edges, and agents
/// </summary>
public class PoolManager : MonoBehaviour
{
    //All variables
    private List<GameObject> edges;
    private List<GameObject> nodes;
    private List<NPCAgent> agents;

    public int maxNodesAllowed = 10000;
    public int maxEdgesAllowed = 10000;

    public GameObject agentPrefab;
    public GameObject nodePrefab;
    public GameObject linePrefab;


    //Singleton design pattern
    public static PoolManager Instance { get; private set; }
    //Init function
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        agents = new List<NPCAgent>();
        edges = new List<GameObject>();
        nodes = new List<GameObject>();

        StartCoroutine(Setup());
    }

    public IEnumerator Setup()
    {
        List<GameObject> newEdges = new List<GameObject>();
        List<GameObject> newNodes = new List<GameObject>();

        for (int i = 0; i < 100000; i++)
        {
            newEdges.Add(GetEdge());
            newNodes.Add(GetNode());
            yield return new WaitForSeconds(0.01f);
        }

        for (int i = 0; i < 100000; i++)
        {
            newEdges[i].SetActive(false);
            newNodes[i].SetActive(false);
        }

    }



    //Return agent from pool or create new one if all are being used
    public NPCAgent GetAgent()
    {
        foreach (var agent in agents)
        {
            if (!agent.gameObject.activeSelf)
            {
                agent.EnableAgent();
                return agent;  // Return an inactive node to reuse
            }
        }

        GameObject newNPCGO = Instantiate(agentPrefab,GameManager.Instance.GetRandomNodeOnGraph(false).GetPosition(),Quaternion.identity);
        NPCAgent npc = newNPCGO.GetComponent<NPCAgent>();   

        agents.Add(npc);
        return npc;
    }

    //Return node from pool or create new one if all are being used
    public GameObject GetNode()
    {
        foreach (var node in nodes)
        {
            if (!node.activeSelf)
            {
                node.SetActive(true);
                return node; 
            }
        }

        GameObject newNodeGO = Instantiate(nodePrefab);
        nodes.Add(newNodeGO);
        return newNodeGO;
    }
    //Return edge from pool or create new one if all are being used
    public GameObject GetEdge()
    {
        foreach (var edge in edges)
        {
            if (!edge.activeSelf)
            {
                edge.SetActive(true);
                return edge;  
            }
        }
        GameObject newEdgeGO = Instantiate(linePrefab,Vector3.zero,Quaternion.identity);
        edges.Add(newEdgeGO);
        return newEdgeGO;
    }

}
